using System;

namespace dashboard.web.Providers
{
    public interface IDateProvider
    {
        Timestamp Now { get; }
        string NowString { get; }
        Timestamp In(TimeSpan timeSpan);
    }

    public class UtcDateProvider : IDateProvider
    {
        private DateTime _now => DateTime.UtcNow;

        public Timestamp Now => new Timestamp(_now);
        public string NowString => _now.ToString();
        public Timestamp In(TimeSpan timeSpan) => new Timestamp(_now.Add(timeSpan));
    }

    public class Timestamp
    {
        private readonly DateTime _timestamp;

        internal Timestamp(DateTime timestamp)
        {
            _timestamp = timestamp;
        }

        public override string ToString()
        {
            return _timestamp.ToString();
        }

        public static implicit operator DateTime(Timestamp timestamp)
        {
            return timestamp._timestamp;
        }
    }
}