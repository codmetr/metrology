using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.EventAggregator
{
    internal class EventSubscriber : IEquatable<EventSubscriber>
    {
        public Type SubscriberType { get; set; }

        public object Token { get; set; }

        public override int GetHashCode()
        {
            return SubscriberType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventSubscriber);
        }

        public bool Equals(EventSubscriber obj)
        {
            return obj.SubscriberType == SubscriberType && obj.Token == Token;
        }
    }
}
