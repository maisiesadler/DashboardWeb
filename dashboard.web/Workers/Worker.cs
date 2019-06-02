using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dashboard.web.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ServiceWorkerSubscriptionManager _subscriptionManager;
        private readonly IRemindersProvider _remindersProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(
            ServiceWorkerSubscriptionManager subscriptionManager,
            IRemindersProvider remindersProvider,
            ILogger<Worker> logger)
        {
            _subscriptionManager = subscriptionManager;
            _remindersProvider = remindersProvider;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTime.Now}.");
                var triggerable = _remindersProvider.GetTriggerable();
                if (triggerable.Any())
                {
                    _subscriptionManager.Push(string.Join(",", triggerable.Select(t => t.ReminderText)));
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}