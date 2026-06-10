using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {

        private ITopicClient topicClient;
        private ManagementClient managementClient;
        private ILogger logger;
        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
            managementClient = new ManagementClient(config.EventBusConnectionString);
            topicClient = CreateTopicClient();
        }

        private ITopicClient CreateTopicClient()
        {
            if (topicClient != null || topicClient.IsClosedOrClosing)
            {
                return topicClient;
            }
            if(!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            {
                managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
            }
            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);
            var eventJson = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventJson);
            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = eventName
            };
            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if(!SubsManager.HasSubscriptionForEvent(eventName))
            {
                var subClient = CreateSubscriptionClientIfNotExist(eventName);
                RegisterSubscriptionClientMessageHandler(subClient);
            }
            logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                var subClient = CreateSubscriptionClient(eventName);
                subClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult();

            }
            catch (MessagingEntityNotFoundException)
            {
                logger.LogWarning("The messaging entity {0} was not found.", eventName);
            }
            logger.LogInformation("Unsubscribing from event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            SubsManager.RemoveSubscription<T, TH>();

        }
        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subClient)
        {
            subClient.RegisterMessageHandler(async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);
               if(await ProcessEvent(ProcessEventName(eventName), messageData))
                {
                    await subClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            });
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            var ex = arg.Exception;
            var context = arg.ExceptionReceivedContext;

            logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);
            return Task.CompletedTask;
        }
        public void CreateRuleIfNotExist(string eventName, ISubscriptionClient subClient)
        {
            bool ruleExists = false;

            try
            {
                var rule = managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName).GetAwaiter().GetResult();
                ruleExists = rule != null;
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                ruleExists = false;
            }
            if (!ruleExists)
            {
                subClient.AddRuleAsync(new RuleDescription()
                {
                    Name = eventName,
                    Filter = new CorrelationFilter() { Label = eventName }
                }).GetAwaiter().GetResult();
            }
        }
        private void RemoveDefaultRule(SubscriptionClient subClient)
        {
            try
            {
                subClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
               logger.LogWarning("The messaging entity {0} was not found.", RuleDescription.DefaultRuleName);
            }
        }
        private ISubscriptionClient CreateSubscriptionClientIfNotExist(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);
            var exists = managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
            if (!exists) {
                managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }
            CreateRuleIfNotExist(ProcessEventName(eventName), subClient);
            return subClient;
        }
        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
        }
        public override void Dispose()
        {
            base.Dispose();
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managementClient = null;
        }
    }
}
