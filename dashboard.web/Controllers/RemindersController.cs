using System;
using dashboard.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace dashboard.web.Controllers
{
    [Route("[controller]")]
    public class RemindersController : Controller
    {
        private readonly IRemindersProvider _remindersProvider;

        public RemindersController(IRemindersProvider remindersProvider)
        {
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

            var remindAt = DateTime.UtcNow.AddSeconds(int.Parse(reminderAddModel.Time));
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
