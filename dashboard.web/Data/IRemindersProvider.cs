using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using dashboard.web.Providers;

namespace dashboard.web
{
    public class Reminder
    {
        public Timestamp Date { get; }
        public string ReminderText { get; }

        public Reminder(Timestamp date, string reminderText)
        {
            Date = date;
            ReminderText = reminderText;
        }
    }

    public interface IRemindersProvider
    {
        long Add(Reminder reminder);
        void Complete(long id);
        List<Triggerable<Reminder>> Get();
        List<DbItem<Reminder>> GetTriggerable();
        void MarkAsTriggered(long id);
        object SyncRoot { get; }
    }

    public class RemindersProvider : IRemindersProvider
    {
        TriggerableInMemDatabase<Reminder> _database = new TriggerableInMemDatabase<Reminder>();

        private object _syncRoot = new object();
        private readonly IDateProvider _dateProvider;

        public object SyncRoot => _syncRoot;

        public RemindersProvider(IDateProvider dateProvider)
        {
            _dateProvider = dateProvider;
        }

        public long Add(Reminder reminder)
        {
            lock (_syncRoot)
            {
                return _database.Add(reminder);
            }
        }

        public void Complete(long id)
        {
            lock (_syncRoot)
            {
                _database.Close(id);
            }
        }

        public List<Triggerable<Reminder>> Get()
        {
            return _database.Get();
        }

        // assumes lock is taken elsewhere
        public List<DbItem<Reminder>> GetTriggerable()
        {
            var now = _dateProvider.Now;
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