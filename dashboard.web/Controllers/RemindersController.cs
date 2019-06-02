using System;
using dashboard.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace dashboard.web.Controllers
{
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly IRemindersProvider _remindersProvider;

        public RemindersController(IRemindersProvider remindersProvider)
        {
            _remindersProvider = remindersProvider;
        }

        [HttpPost]
        public IActionResult Add([Bind("Time", "ReminderText")] AddReminderModel addReminderModel)
        {
            if (addReminderModel == null || string.IsNullOrWhiteSpace(addReminderModel.ReminderText)) return BadRequest();
            
            var remindAt = DateTime.UtcNow.AddSeconds(int.Parse(addReminderModel.Time));
            _remindersProvider.Add(new Reminder(remindAt, addReminderModel.ReminderText));
            return RedirectToAction("Index", "Home");
        }
    }
}
