
using System;
using System.Security.Authentication.ExtendedProtection;
using TelemetrySystem;

namespace TelemetrySystem
{
    public class Event
    {
        public readonly EventType Type;
        protected long _timeStamp;
        public long TimeStamp { get { return _timeStamp; } }

        public static bool Active;

        public Event(EventType type, DateTimeOffset time)
        {
            Type = type;
            _timeStamp = time.ToUnixTimeMilliseconds();
            
            // De momento lo dejamos a True, pero aquí habría que
            // llamar al Tracker para saber si este tipo de evento está permitido.
            Active = true;
        }

        public string ToJSON()
        {
            string message = "\"event_type\": \"" + Type.ToString() + '\"'
                + "\"time_stamp\": \"" + TimeStamp.ToString() + "\"";

            return message;
        }
    }

    public abstract class PersistentEvent : Event
    {
        // en ms
        public readonly int PersistentTime;
        public long _currentPersistentTime;

        public abstract void GetDataCallback();

        public void UpdatePersistentTime()
        {
            _currentPersistentTime = DateTimeOffset.UtcNow.AddTicks(PersistentTime * 10000).ToUnixTimeMilliseconds();
        }

        public void UpdateTimeStamp()
        {
            this._timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public long AdvanceTimer()
        {
            _currentPersistentTime += PersistentTime;
            return _currentPersistentTime;
        }

        public PersistentEvent(EventType type, int persistencyTime) : base(type, DateTimeOffset.UtcNow)
        {
            PersistentTime = persistencyTime;
            _currentPersistentTime = persistencyTime;
        }
    }
}

public class MiauEvent : TelemetrySystem.PersistentEvent
{
    public MiauEvent(EventType type, int persistencyTime) : base(type, persistencyTime)
    {
    }

    public override void GetDataCallback()
    {
        throw new System.NotImplementedException();
    }
}