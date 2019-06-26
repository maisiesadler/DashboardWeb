using System;
using System.Threading.Tasks;
using dashboard.web.Models;
using dashboard.web.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace dashboard.web.Controllers
{
    [Route("[controller]")]
    public class RemindersController : Controller
    {
        private readonly IDateProvider _dateProvider;
        private readonly IRemindersProvider _remindersProvider;
        private readonly ILogger<RemindersController> _logger;

        public RemindersController(
            IDateProvider dateProvider,
            IRemindersProvider remindersProvider,
            ILogger<RemindersController> logger)
        {
            _dateProvider = dateProvider;
            _remindersProvider = remindersProvider;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var reminders = await _remindersProvider.ViewActiveAsync();
            return View(new RemindersViewModel(reminders));
        }

        [HttpPost]
        public async Task<IActionResult> Add([Bind("Time", "ReminderText")] ReminderAddModel reminderAddModel)
        {
            if (reminderAddModel == null || string.IsNullOrWhiteSpace(reminderAddModel.ReminderText)) return BadRequest();

            var remindAt = _dateProvider.In(TimeSpan.FromSeconds(int.Parse(reminderAddModel.Time)));
            await _remindersProvider.AddAsync(new Reminder(remindAt, reminderAddModel.ReminderText));
            return RedirectToAction("Index");
        }

        [HttpPut("done")]
        public async Task<IActionResult> Done(ReminderCompletedModel reminderCompletedModel)
        {
            _logger.LogInformation("closing " + reminderCompletedModel.Id);
            if (reminderCompletedModel == null) return BadRequest();

            await _remindersProvider.CompleteAsync(new ObjectId(reminderCompletedModel.Id));
            return Ok();
        }
    }
}
