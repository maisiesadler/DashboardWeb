using Microsoft.AspNetCore.Mvc;

namespace dashboard.web.Controllers
{
    [Route("api/[controller]")]
    public class ServiceWorkerController : ControllerBase
    {
        private readonly ServiceWorkerSubscriptionManager _subscriptionManager;

        public ServiceWorkerController(ServiceWorkerSubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        [HttpGet]
        public IActionResult GetAction()
        {
            return Ok();
        }

        [HttpGet("/api/[controller]/key")]
        public IActionResult Key()
        {
            return Ok(_subscriptionManager.PublicKey);
        }

        [HttpPost]
        public IActionResult PostAction([FromBody] ServiceWorkerSubscription serviceWorkerSubscription)
        {
            _subscriptionManager.Set(serviceWorkerSubscription);
            return Ok();
        }
    }
}
