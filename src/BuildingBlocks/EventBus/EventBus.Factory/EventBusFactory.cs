using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;
using System;
using static EventBus.Base.EventBusConfig;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
           return config.EventBusType switch
           {
               EventBusTypes.RabbitMQ => new EventBusRabbitMQ(config, serviceProvider),
               EventBusTypes.AzureServiceBus => new EventBusServiceBus(config, serviceProvider),
               _ => throw new ArgumentException("Invalid event bus type")
           };
        }
    }
}
