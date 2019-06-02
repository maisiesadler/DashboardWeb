using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using WebPush;

namespace dashboard.web
{
    // https://developers.google.com/web/fundamentals/codelabs/push-notifications/
    // https://web-push-codelab.glitch.me
    // https://github.com/web-push-libs/
    public class ServiceWorkerSubscription
    {
        public string endpoint { get; set; }
        public ServiceWorkerSubscriptionKeys keys { get; set; }
    }

    public class ServiceWorkerSubscriptionKeys
    {
        public string auth { get; set; }
        public string p256dh { get; set; }
    }

    public class ServiceWorkerSubscriptionManager
    {
        private ServiceWorkerSubscription _subscription;
        private readonly ILogger<ServiceWorkerSubscriptionManager> _logger;
        private const string DASHBOARD_KEYS_PUBLIC = "DASHBOARD_KEYS_PUBLIC";
        private const string DASHBOARD_KEYS_PRIVATE = "DASHBOARD_KEYS_PRIVATE";
        private string _publicKey;
        private string _privateKey;
        private bool _healthy = false;

        public ServiceWorkerSubscriptionManager(ILogger<ServiceWorkerSubscriptionManager> logger)
        {
            _logger = logger;

            _publicKey = Environment.GetEnvironmentVariable(DASHBOARD_KEYS_PUBLIC);
            _privateKey = Environment.GetEnvironmentVariable(DASHBOARD_KEYS_PRIVATE);

            if (string.IsNullOrWhiteSpace(_publicKey) ||
                string.IsNullOrWhiteSpace(_privateKey))
            {
                _logger.LogWarning("Cannot find public/private key.");
            }
            else
            {
                _healthy = true;
            }
        }

        public void Set(ServiceWorkerSubscription subscription)
        {
            _subscription = subscription;
        }

        public bool Push(string message)
        {
            if (!_healthy)
            {
                _logger.LogError("Service worker not healthy");
                return false;
            }
            if (_subscription == null)
            {
                _logger.LogWarning("No subscription");
                return false; ;
            }

            var subject = @"mailto:example@example.com";

            var subscription = new PushSubscription(_subscription.endpoint, _subscription.keys.p256dh, _subscription.keys.auth);
            var vapidDetails = new VapidDetails(subject, _publicKey, _privateKey);
            //var gcmAPIKey = @"[your key here]";

            var webPushClient = new WebPushClient();
            try
            {
                webPushClient.SendNotification(subscription, message, vapidDetails);
                //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
                return true;
            }
            catch (WebPushException exception)
            {
                _logger.LogError("Http STATUS code" + exception.StatusCode);
                return false;
            }
        }

        public string PublicKey => _publicKey;
    }
}