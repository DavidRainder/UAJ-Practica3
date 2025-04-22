using System;
using System.Xml;

namespace TelemetrySystem
{
    public abstract class TrackerEvent
    {
        public string eventType;
        protected long timeStamp;
        public long TimeStamp { get { return timeStamp; } }

        public TrackerEvent(DateTimeOffset time)
        {
            timeStamp = time.ToUnixTimeMilliseconds();
        }
        public abstract string GetID();

        public virtual string ToJSON()
        {
            return $"\"event_type\": \"{GetID()}\", \"time_stamp\": \"{TimeStamp.ToString()}\"";
        }

        public virtual string ToXML(XmlDocument doc, XmlNode eventsNode, out XmlNode myEvent)
        {
            myEvent = doc.CreateElement(GetID());
            eventsNode.AppendChild(myEvent);

            XmlAttribute timeStamp = doc.CreateAttribute("timestamp");
            timeStamp.Value = TimeStamp.ToString();
            myEvent.Attributes.Append(timeStamp);
            return myEvent.OuterXml;
        }
    }

    public abstract class TrackerPersistentEvent : TrackerEvent
    {
        // en ms
        /// <summary>
        /// Tiempo que pasa hasta que el evento ocurre.
        /// </summary>
        public readonly int PersistentTime;
        /// <summary>
        /// El tiempo que lleva acumulado
        /// Se ha sucedido 1 vez, será PersistentTime. 
        /// Si se sucedido 2 veces, será 2*PersistenTime, etc.
        /// </summary>
        public long _currentPersistentTime;

        public TrackerPersistentEvent(int persistencyTime) : base(DateTimeOffset.UtcNow)
        {
            PersistentTime = persistencyTime;
            _currentPersistentTime = DateTimeOffset.UtcNow.AddMilliseconds(persistencyTime).ToUnixTimeMilliseconds();
        }
        public abstract void GetDataCallback();

        public void UpdatePersistentTime()
        {
            _currentPersistentTime = DateTimeOffset.UtcNow.AddMilliseconds(PersistentTime).ToUnixTimeMilliseconds();
        }

        public void UpdateTimeStamp()
        {
            timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public long AdvanceTimer()
        {
            _currentPersistentTime += PersistentTime;
            return _currentPersistentTime;
        }
    }
}
