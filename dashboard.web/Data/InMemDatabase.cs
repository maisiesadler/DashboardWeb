using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace dashboard.web
{
    public struct Triggerable<T>
    {
        public DbItem<T> DbItem { get; }
        public T Value => DbItem.Value;
        public bool Triggered { get; set; }

        public Triggerable(DbItem<T> value)
        {
            DbItem = value;
            Triggered = false;
        }
    }

    public struct DbItem<T>
    {
        public long Id { get; }
        public T Value { get; }

        public DbItem(long id, T value)
        {
            Id = id;
            Value = value;
        }
    }

    public class TriggerableInMemDatabase<T>
    {
        protected Dictionary<long, Triggerable<T>> _data = new Dictionary<long, Triggerable<T>>();
        protected Dictionary<long, T> _closed = new Dictionary<long, T>();
        private long _nextId = 0;

        public long Add(T t)
        {
            var next = Interlocked.Increment(ref _nextId);
            _data[next] = new Triggerable<T>(new DbItem<T>(next, t));
            return next;
        }

        public List<Triggerable<T>> Get() => _data.Values
                                    .ToList();

        public Triggerable<T> Get(long id)
        {
            if (_data.TryGetValue(id, out var triggered))
                return triggered;

            throw new Exception("Cannot find item with id " + id);
        }

        public void SetTriggered(long id, bool triggered)
        {
            var m = Get(id);
            m.Triggered = true;
            _data[id] = m;
        }
    }
}