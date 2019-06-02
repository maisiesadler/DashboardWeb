using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace dashboard.web
{
    public class InMemDatabase<T>
    {
        public Dictionary<long, T> _data = new Dictionary<long, T>();
        private long _nextId = 0;

        public long Add(T t)
        {
            var next = Interlocked.Increment(ref _nextId);
            _data[next] = t;
            return next;
        }

        public List<T> Get() => _data.Values.ToList();
    }
}