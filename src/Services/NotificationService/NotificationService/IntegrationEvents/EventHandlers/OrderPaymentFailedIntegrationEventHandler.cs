using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    internal class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            _logger.LogInformation($"Order payment failed for order ID: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");
            return Task.CompletedTask;
        }
    }
}
