using System;
using dashboard.web.Models;
using dashboard.web.Providers;
using Microsoft.AspNetCore.Mvc;

namespace dashboard.web.Controllers
{
    [Route("[controller]")]
    public class RemindersController : Controller
    {
        private readonly IDateProvider _dateProvider;
        private readonly IRemindersProvider _remindersProvider;

        public RemindersController(
            IDateProvider dateProvider,
            IRemindersProvider remindersProvider)
        {
            _dateProvider = dateProvider;
            _remindersProvider = remindersProvider;
        }

        public IActionResult Index()
        {
            var reminders = _remindersProvider.Get();
            return View(new RemindersViewModel(reminders));
        }

        [HttpPost]
        public IActionResult Add([Bind("Time", "ReminderText")] ReminderAddModel reminderAddModel)
        {
            if (reminderAddModel == null || string.IsNullOrWhiteSpace(reminderAddModel.ReminderText)) return BadRequest();

            var remindAt = _dateProvider.In(TimeSpan.FromSeconds(int.Parse(reminderAddModel.Time)));
            _remindersProvider.Add(new Reminder(remindAt, reminderAddModel.ReminderText));
            return RedirectToAction("Index");
        }

        [HttpPut("done")]
        public IActionResult Done(ReminderCompletedModel reminderCompletedModel)
        {
            if (reminderCompletedModel == null) return BadRequest();

            _remindersProvider.Complete(reminderCompletedModel.Id);
            return Ok();
        }
    }
}
