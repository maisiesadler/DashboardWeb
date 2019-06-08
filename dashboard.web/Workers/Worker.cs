using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dashboard.web.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dashboard.web.Workers
{
    public class Worker : BackgroundService
    {
        private readonly IDateProvider _dateProvider;
        private readonly ServiceWorkerSubscriptionManager _subscriptionManager;
        private readonly IRemindersProvider _remindersProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IDateProvider dateProvider,
            ServiceWorkerSubscriptionManager subscriptionManager,
            IRemindersProvider remindersProvider,
            ILogger<Worker> logger)
        {
            _dateProvider = dateProvider;
            _subscriptionManager = subscriptionManager;
            _remindersProvider = remindersProvider;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {_dateProvider.NowString}.");
                lock (_remindersProvider.SyncRoot)
                {
                    var triggerable = _remindersProvider.GetTriggerable();
                    if (triggerable.Any())
                    {
                        foreach(var t in triggerable)
                        {
                            _remindersProvider.MarkAsTriggered(t.Id);
                        }
                        _subscriptionManager.Push(string.Join(",", triggerable.Select(t => t.Value.ReminderText)));
                    }
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}