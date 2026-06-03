using System;

namespace EventBus.Base.SubManagers
{
    // Minimal SubscriptionInfo expected by InMemoryEventBusSubscriptionManager
    public class SubscriptionInfo
    {
        public Type HandlerType { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public SubscriptionInfo()
        {
            CreatedAt = DateTime.UtcNow;
        }

        private SubscriptionInfo(Type handlerType) : this()
        {
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}
