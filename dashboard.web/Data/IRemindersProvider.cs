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
        List<Triggerable<Reminder>> Get();
        List<DbItem<Reminder>> GetTriggerable();
        void MarkAsTriggered(long id);
        object SyncRoot { get; }
    }

    public class RemindersProvider : IRemindersProvider
    {
        TriggerableInMemDatabase<Reminder> _database = new TriggerableInMemDatabase<Reminder>();

        private object _syncRoot = new object();
        public object SyncRoot => _syncRoot;

        public long Add(Reminder reminder)
        {
            lock (_syncRoot)
            {
                return _database.Add(reminder);
            }
        }

        public List<Triggerable<Reminder>> Get()
        {
            return _database.Get();
        }

        // assumes lock is taken elsewhere
        public List<DbItem<Reminder>> GetTriggerable()
        {
            var now = DateTime.UtcNow;
            var triggerable = new List<DbItem<Reminder>>();
            foreach (var item in _database.Get())
            {
                if (!item.Triggered && item.Value.Date <= now)
                {
                    triggerable.Add(item.DbItem);
                }
            }
            return triggerable;
        }

        // assumes lock is taken elsewhere
        public void MarkAsTriggered(long id)
        {
            _database.SetTriggered(id, true);
        }
    }
}