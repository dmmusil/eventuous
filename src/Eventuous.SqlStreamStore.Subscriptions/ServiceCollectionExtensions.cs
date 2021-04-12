using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eventuous.SqlStreamStore.Subscriptions {
    public static class ServiceCollectionExtensions {
        public static void AddSubscription<T>(
            this IServiceCollection services
        ) where T : SubscriptionService {
            services.AddSingleton<T>();
            services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<T>());
        }

        public static void AddSubscription<T>(
            this IServiceCollection services, string checkName, string[] tags
        ) where T : SubscriptionService {
            services.AddSubscription<T>();
            services.AddHealthChecks().AddCheck<T>(checkName, null, tags);
        }
    }
}