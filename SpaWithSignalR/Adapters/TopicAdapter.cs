using Application.Frontend.Controllers;
using Messaging.Abstractions;
using Messaging.Azure.ServiceBus.Subscriber;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Frontend.Adapters
{
    public abstract class TopicAdapter<T> : IMessageReceivedHandler<string>
    {

        private readonly ISubscriber<string> topicSubscriber;
        IHubContext<BroadcastHub, IHubClient> hubContext;
        
        public TopicAdapter(ISubscriber<string> topicSubscriber, ILogger<TopicAdapter<T>> logger) 
        {
            this.topicSubscriber = topicSubscriber;

        }

        public virtual async Task<MessageReceivedHandleResult> HandleAsync(MessageReceived<string> message, CancellationToken token = default)
        {
            T eventPayload = JsonConvert.DeserializeObject<T>(message.Payload);

            await DispatchEventAsync(eventPayload);
            
            return new MessageReceivedHandleResult(HandleMode.Sync, true);
        }

        protected abstract Task DispatchEventAsync(T eventPayload);

        public async Task SetupAsync() 
        {
            await topicSubscriber.SubscribeAsync(this, new MessageOptions("WeatherForecastFE", 1), default);
        }
    }
}
