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

        public Timestamp Now => new Timestamp(_now.Ticks);
        public string NowString => _now.ToString();
        public Timestamp In(TimeSpan timeSpan) => new Timestamp(_now.Add(timeSpan).Ticks);
    }

    public class Timestamp
    {
        private readonly long _timestamp;

        internal Timestamp(long timestamp)
        {
            _timestamp = timestamp;
        }

        public override string ToString()
        {
            return new DateTime(_timestamp).ToString();
        }

        public static implicit operator long(Timestamp timestamp)
        {
            return timestamp._timestamp;
        }
    }
}