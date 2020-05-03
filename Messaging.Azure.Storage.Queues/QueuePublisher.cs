using Messaging.Abstractions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Azure.Storage.Queues
{
    /// <summary>
    /// Crreates an instance of QueuePublisher
    /// </summary>
    public class QueuePublisher : IPublisher<string>
    {
       
        private ConnectionSettings publisherSettings;
        private LogLevel logLevel;
        private CloudQueueClient cloudClient;
        private CloudQueue cloudQueue;
        private MessageOptions messageOptions;
        private QueueRequestOptions queueRequestOptions;
        private OperationContext operationContext;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textSerializer"></param>
        /// <param name="publisherSettings"></param>
        /// <param name="serializationSettings"></param>
        /// <param name="logLevel"></param>
        public QueuePublisher(ConnectionSettings publisherSettings, LogLevel logLevel = LogLevel.Error)
        {
            
            this.publisherSettings = publisherSettings;
            this.logLevel = logLevel;
            
            var storageAccount = CloudStorageAccount.Parse(this.publisherSettings.ConnectionString);
            cloudClient = storageAccount.CreateCloudQueueClient();

            if (string.IsNullOrEmpty(publisherSettings.Path))
            {
                throw new ArgumentNullException(nameof(publisherSettings.Path));
            }

            cloudQueue = cloudClient.GetQueueReference(publisherSettings.Path);
         

        }

        ///
        public QueuePublisher(MessageOptions messageOptions, ConnectionSettings publisherSettings, LogLevel logLevel = LogLevel.Error) 
            : this( publisherSettings,  logLevel)
        {
           
            this.messageOptions = messageOptions;
            queueRequestOptions = new QueueRequestOptions() { MaximumExecutionTime = messageOptions.MaximumExecutionTime, ServerTimeout = messageOptions.ServerTimeout };
            operationContext = new OperationContext() { ClientRequestID = messageOptions.ClientId, CustomUserAgent = messageOptions.CustomUserAgent, UserHeaders = messageOptions.UserHeaders, LogLevel = logLevel };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="queueRequestOptions"></param>
        /// <param name="operationContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async Task<PublishResult> PublishAsync(string payload, string path, MessageOptions options, QueueRequestOptions queueRequestOptions, OperationContext operationContext, CancellationToken token)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var message = await CreateCloudMessage(payload);
           

            CloudQueue scopedQueue = this.cloudQueue;
            if (!string.IsNullOrEmpty(path))
            {
                scopedQueue = cloudClient.GetQueueReference(path);
            }

            await scopedQueue.CreateIfNotExistsAsync();

            await scopedQueue.AddMessageAsync(message, options.TimeToLive, options.InitialVisibilityDelay, queueRequestOptions, operationContext, token);
                        
            return new PublishResult(message.Id, message.ExpirationTime, message.InsertionTime, message.PopReceipt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected virtual async Task<CloudQueueMessage> CreateCloudMessage(string payload)
        {   
            var message = new CloudQueueMessage(payload);
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<PublishResult> PublishAsync(string payload, string path, MessageOptions options, CancellationToken token)
        {

            var queueRequestOptions = new QueueRequestOptions() { MaximumExecutionTime = options.MaximumExecutionTime, ServerTimeout = options.ServerTimeout };
            var operationContext = new OperationContext() { ClientRequestID = options.ClientId, CustomUserAgent = options.CustomUserAgent, UserHeaders = options.UserHeaders, LogLevel = logLevel };

            return await PublishAsync(payload, path, options, queueRequestOptions, operationContext, token);

        }




    }
}
