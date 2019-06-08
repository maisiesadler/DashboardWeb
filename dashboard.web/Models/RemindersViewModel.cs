using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dashboard.web.Models
{
    public class RemindersViewModel
    {
        public RemindersViewModel(List<Triggerable<Reminder>> reminders)
        {
            Reminders = reminders;
            Options = new List<SelectListItem>()
            {
                new SelectListItem("10 secs", "10"),
                new SelectListItem("30 secs", "30"),
                new SelectListItem("1 minute", "60"),
                new SelectListItem("2 mins", "120"),
            };
        }

        public List<Triggerable<Reminder>> Reminders { get; set; }
        public List<SelectListItem> Options { get; set; }
        public int ReminderCount => Reminders.Count;
    }
}