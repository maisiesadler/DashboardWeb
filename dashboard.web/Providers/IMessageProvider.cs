using System.Collections.Generic;

namespace dashboard.web.Providers
{
    public class IMessageProvider
    {
        public List<Message> GetTriggerable()
        {
            return new List<Message> { };
        }
    }

    public class Message
    {
        public string Contents { get; }
    }
}