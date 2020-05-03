using Application.Abstractions;
using Application.Abstractions.Apis;
using Application.Frontend.Controllers;
using Messaging.Azure.ServiceBus.Subscriber;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend.Adapters
{
    public class RecomendationsTopicAdapter : TopicAdapter<WeatherRecomendation> 
    {
        private readonly IRecomendationsRepository recomendationRepository;
        private readonly IHubContext<BroadcastHub, IHubClient> hubContext;

        public RecomendationsTopicAdapter(ISubscriber<string> topicSubscriber, IHubContext<BroadcastHub, IHubClient> hubContext, IRecomendationsRepository recomendationRepository, ILogger<RecomendationsTopicAdapter> logger) 
            : base(topicSubscriber, logger)
        {
            this.recomendationRepository = recomendationRepository;
            this.hubContext = hubContext;

        }

        protected override async Task DispatchEventAsync(WeatherRecomendation eventPayload)
        {
            recomendationRepository.AddOrUpdate(eventPayload);
            await hubContext.Clients.All.BroadcastRecomendations(eventPayload);
        }
    }
}
