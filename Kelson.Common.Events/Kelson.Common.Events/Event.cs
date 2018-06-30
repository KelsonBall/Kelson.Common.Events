using System.Collections.Generic;

namespace Kelson.Common.Events
{
    public class Event
    {
        public readonly Dictionary<string, object> Metadata = new Dictionary<string, object>();
    }

    public class Event<T> : Event
    {
        public T Args { get; set; }
    }
}
