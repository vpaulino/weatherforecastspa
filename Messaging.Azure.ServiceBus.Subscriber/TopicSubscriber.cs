using Messaging.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Messaging.Azure.ServiceBus.Subscriber
{
    /*
    
        MaxAutoRenewDuration Explanation
     There are a few things you need to consider.

Lock duration
Total time since a message acquired from the broker
The lock duration is simple - for how long a single competing consumer can lease a message w/o having that message leased to any other competing consumer.

The total time is a bit tricker. Your callback ProcessMessagesAsync registered with to receive the message is not the only thing that is involved. 
In the code sample, you've provided, you're setting the concurrency to 1. 
If there's a prefetch configured (queue gets more than one message with every request for a message or several), the lock duration clock on the server starts ticking for all those messages. 
So if your processing is done slightly under MaxLockDuration but for the same of example, the last prefetched message was waiting to get processed too long, 
even if it's done within less than lock duration time, it might lose its lock and the exception will be thrown when attempting completion of that message.
This is where MaxAutoRenewDuration comes into the game. What it does is extends the message lease with the broker, 
"re-locking" it for the competing consumer that is currently handling the message. 
MaxAutoRenewDuration should be set to the "possibly maximum processing time a lease will be required". 
It needs to be set to be at least longer than the MaxLockDuration and adjusted to the longest period of time ProccesMessage will need to run. Taking prefetching into consideration.
Think of the client-side having an in-memory queue where the messages can be stored while you perform the serial processing of the messages one by one in your handler. 
Lease starts the moment a message arrives from the broker to that in-memory queue. If the total time in the in-memory queue plus the processing exceeds the lock duration, the lease is lost. 
Your options are:

Enable concurrent processing by setting MaxConcurrentCalls > 1
Increase MaxLockDuration
Reduce message prefetch (if you use it)
Configure MaxAutoRenewDuration to renew the lock and overcome the MaxLockDuration constraint
Note about #4 - it's not a guaranteed operation. Therefore there's a chance a call to the broker will fail and message lock will not be extended.
I recommend designing your solutions to work within the lock duration limit.
Alternatively, persist message information so that your processing doesn't have to be constrained by the messaging.
         */
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TopicSubscriber : ISubscriber<string>,  IMessageReceivedCallback<string>
    {
        private ConnectionSettings connectionSettings;
        private SubscriptionClient subscriptionClient;
        private IMessageReceivedHandler<string> messageHandler;
        private ILogger logger;
        private Guid subscriptionId;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connection"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="textSerializer"></param>
        /// <param name="serializationSettings"></param>
        public TopicSubscriber(ILogger<TopicSubscriber> logger, ConnectionSettings connection)
        {
            this.logger = logger;
            this.connectionSettings = connection;
            this.subscriptionClient = new SubscriptionClient(this.connectionSettings.ConnectionString, this.connectionSettings.Path, connection.SubscriptionName, ReceiveMode.PeekLock);
        }

        private Task PickUpAndFixDeadletters(string connectionString, string queueName, CancellationToken cancellationToken)
        {
            var doneReceiving = new TaskCompletionSource<bool>();

            // here, we create a receiver on the Deadletter queue
            var dlqReceiver = new MessageReceiver(connectionString, EntityNameHelper.FormatDeadLetterPath(queueName), ReceiveMode.PeekLock);

            // close the receiver and factory when the CancellationToken fires 
            cancellationToken.Register(
                async () =>
                {
                    await dlqReceiver.CloseAsync();
                    doneReceiving.SetResult(true);
                });

            // register the RegisterMessageHandler callback
            dlqReceiver.RegisterMessageHandler(
                async (message, cancellationToken1) =>
                {
                    // first, we create a clone of the picked up message
                    // that we can resubmit. 
                    var resubmitMessage = message.Clone();
                  
                    // finally complete the original message and remove it from the DLQ
                    await dlqReceiver.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });

            return doneReceiving.Task;
        }

        Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            this.logger.LogError("Exception: \"{0}\" {0}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task CancelSubscriptionAsync(Subscription subscription)
        {
            if (subscription.Id is Guid && this.subscriptionId != (Guid)subscription.Id)
                return;

            await this.subscriptionClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages(MessageOptions options)
        {
            
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = options.ConcurrentCalls,
                MaxAutoRenewDuration = options.MaximumExecutionTime.Value,
                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {

            this.logger.LogError($"Exception: {arg.Exception}, ClientID : {arg.ExceptionReceivedContext.ClientId},Endpoint: {arg.ExceptionReceivedContext.Endpoint}, EntityPath : {arg.ExceptionReceivedContext.EntityPath}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageHandler"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Subscription> SubscribeAsync(IMessageReceivedHandler<string> messageHandler, MessageOptions options, CancellationToken token)
        {
            /* this approach requires share cliend it and client secret from azure resources from the namespace.but following a design principle that resources must be created
             * in advance when deploying environment,the application must log only if suscription is not there
             var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(CLIENT_ID, CLIENT_SECRET, TENANT_ID, AzureEnvironment.AzureGlobalCloud);
                var serviceBusManager = ServiceBusManager.Authenticate(credentials, SUBSCRIPTION_ID);
            serviceBusNamespace = serviceBusManager.Namespaces.GetByResourceGroup(RESOURCE_GROUP, RESOURCE_NAME);
              */

         //   await PickUpAndFixDeadletters(this.connectionSettings.ConnectionString, "dead-letter", token);
            this.messageHandler = messageHandler;
            RegisterOnMessageHandlerAndReceiveMessages(options);
            this.subscriptionId = Guid.NewGuid();
            return new Subscription(this.subscriptionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            logger.LogDebug($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber}");

            if (token.IsCancellationRequested) 
            {
                await this.subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
                return;
            }

            var serializedMessage = Encoding.UTF8.GetString(message.Body);
            var handleResult = await this.messageHandler.HandleAsync(new MessageReceived<string>(message.MessageId, message.SystemProperties.LockToken, null, serializedMessage));
            if (handleResult.Mode == HandleMode.Sync && handleResult.Success) 
            {
                // Complete the message so that it is not received again.
                // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
        }

         


        public async Task SuccessAckAsync(MessageReceived<string> message, CancellationToken token = default)
        {
            await subscriptionClient.CompleteAsync(message.PopReceipt);
        }

        public async Task FailedAckAsync(MessageReceived<string> message, CancellationToken token = default)
        {
            await subscriptionClient.DeadLetterAsync(message.PopReceipt);
        }
    }
}
