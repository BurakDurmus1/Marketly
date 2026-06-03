using System;

namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;
        public string DefaultTopicName { get; set; } = "MarketlyEventBus";
        public string EventBusConnectionString { get; set; } = string.Empty;
        public string SubscriberClientAppName { get; set; } = string.Empty;
        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        public string EventNamePrefix { get; set; } = string.Empty;
        public EventBusTypes EventBusType { get; set; } = EventBusTypes.RabbitMQ;

        public object Connection { get; set; }

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);
        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);

     

        public enum EventBusTypes
        {
            RabbitMQ = 0,
            AzureServiceBus = 1,
            Kafka = 2   
        }
    }
}
