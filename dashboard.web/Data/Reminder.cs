using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace dashboard.web
{
    public class Reminder
    {
        public DateTime Date { get; }
        public string ReminderText { get; }

        public Reminder(DateTime date, string reminderText)
        {
            Date = date;
            ReminderText = reminderText;
        }
    }

    public interface IRemindersProvider
    {
        long Add(Reminder reminder);
        List<Reminder> Get();
        List<Reminder> GetTriggerable();
    }

    public class RemindersProvider : InMemDatabase<Reminder>, IRemindersProvider
    {
        public List<Reminder> GetTriggerable()
        {
            var now = DateTime.UtcNow;
            return _data.Values.Where(d => d.Date <= now).ToList();
        }
    }
}