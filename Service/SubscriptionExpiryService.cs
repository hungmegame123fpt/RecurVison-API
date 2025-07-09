using BusinessObject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SubscriptionExpiryService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionExpiryService> _logger;

        public SubscriptionExpiryService(IServiceProvider serviceProvider, ILogger<SubscriptionExpiryService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime? lastQuotaResetDate = null;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var subscriptionService = scope.ServiceProvider.GetRequiredService<IUserSubscriptionService>();
                    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // 1. Expire old subscriptions
                    var expiredCount = await subscriptionService.ExpireEndedSubscriptionsAsync();
                    if (expiredCount > 0)
                        _logger.LogInformation($"{expiredCount} subscriptions expired at {DateTime.UtcNow}");

                    // 2. Cancel inactive users
                    var cutoffDate = DateTime.UtcNow.AddMonths(-2);
                    var cancelledUsers = await userRepo.CancelUsersNotLoggedInSinceAsync(cutoffDate);

                    if (cancelledUsers > 0)
                        _logger.LogInformation($"{cancelledUsers} users canceled due to 2+ months inactivity at {DateTime.UtcNow}");

                    // 3. Reset quotas once a day (at midnight UTC)
                    var now = DateTime.UtcNow;
                    if (lastQuotaResetDate == null || lastQuotaResetDate.Value.Date != now.Date)
                    {
                        var resetCount = await subscriptionService.ResetUserQuotasAsync(); // create this method
                        _logger.LogInformation($"Reset quotas for {resetCount} subscriptions at {now}");
                        lastQuotaResetDate = now.Date;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in background task.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
