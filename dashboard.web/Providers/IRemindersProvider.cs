using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using dashboard.web.Providers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dashboard.web.Providers
{
    public class Reminder
    {
        public ObjectId Id { get; private set; }

        public DateTime Date { get; private set; }
        public string ReminderText { get; private set; }

        public DateTime? TriggeredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Reminder(Timestamp date, string reminderText)
        {
            Date = date;
            ReminderText = reminderText;
        }
    }

    public interface IRemindersProvider
    {
        Task AddAsync(Reminder reminder);
        Task CompleteAsync(ObjectId id);
        Task<List<Reminder>> ViewActiveAsync();
        Task<List<Reminder>> GetTriggerableAndSetTriggeredAsync();
        Task MarkAsTriggeredAsync(ObjectId id);
    }

    public class RemindersProvider : IRemindersProvider
    {
        private readonly IDateProvider _dateProvider;
        private readonly Lazy<IMongoCollection<Reminder>> _lazyReminders;
        private IMongoCollection<Reminder> _reminders => _lazyReminders.Value;

        public RemindersProvider(
            IDateProvider dateProvider,
            MongoConnection mongoConnection)
        {
            _dateProvider = dateProvider;
            _lazyReminders = new Lazy<IMongoCollection<Reminder>>(() => mongoConnection.Collection<Reminder>("reminders"));
        }

        public async Task AddAsync(Reminder reminder)
        {
            await _reminders.InsertOneAsync(reminder);
        }

        public async Task<List<Reminder>> ViewActiveAsync()
        {
            var activeReminders = await _reminders.FindAsync(x => x.CompletedAt == null);
            return activeReminders.ToList();
        }

        public async Task<List<Reminder>> GetTriggerableAndSetTriggeredAsync()
        {
            var now = _dateProvider.Now;
            var triggerLimit = _dateProvider.In(TimeSpan.FromMinutes(-1));
            var triggerable = (await _reminders.FindAsync(item => item.CompletedAt == null &&
                    item.Date <= now &&
                    (item.TriggeredAt == null || item.TriggeredAt < triggerLimit)))
                    .ToList();

            foreach (var reminder in triggerable)
            {
                await MarkAsTriggeredAsync(reminder.Id);
            }

            return triggerable;
        }

        public async Task CompleteAsync(ObjectId id)
        {
            var now = _dateProvider.Now;
            var update = Builders<Reminder>.Update.Set(e => e.CompletedAt, now);

            var activeReminders = await _reminders.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task MarkAsTriggeredAsync(ObjectId id)
        {
            var now = _dateProvider.Now;
            var update = Builders<Reminder>.Update.Set(e => e.TriggeredAt, now);

            var activeReminders = await _reminders.UpdateOneAsync(x => x.Id == id, update);
        }
    }
}