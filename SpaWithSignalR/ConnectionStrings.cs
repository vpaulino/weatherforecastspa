using Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Frontend
{
    public class ConnectionStrings
    {
        public ConnectionSettings QueuePublisherConnectionSettings { get; set; }
        public ConnectionSettings TopicSubscriberConnectionSettings { get; set; }
    }
}
