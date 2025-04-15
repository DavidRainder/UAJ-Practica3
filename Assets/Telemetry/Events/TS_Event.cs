
using System;
using System.Security.Authentication.ExtendedProtection;
using TelemetrySystem;

namespace TelemetrySystem
{
    public abstract class Event
    {
        public abstract string GetType();
        // public readonly EventType Type;
        protected long _timeStamp;
        public long TimeStamp { get { return _timeStamp; } }

        public static bool Active;

        public Event(DateTimeOffset time)
        {
            _timeStamp = time.ToUnixTimeMilliseconds();
            
            // De momento lo dejamos a True, pero aquí habría que
            // llamar al Tracker para saber si este tipo de evento está permitido.
            Active = true;
        }

        public string ToJSON()
        {
            string message = "\"event_type\": \"" + GetType() + '\"'
                + "\"time_stamp\": \"" + TimeStamp.ToString() + "\"";

            return message;
        }
    }

    public abstract class PersistentEvent : Event
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

        public PersistentEvent(int persistencyTime) : base(DateTimeOffset.UtcNow)
        {
            PersistentTime = persistencyTime;
            _currentPersistentTime = persistencyTime;
        }
    }
}

public class MiauEvent : TelemetrySystem.PersistentEvent
{

    public MiauEvent(int persistencyTime) : base(persistencyTime)
    {
    }

    public override string GetType() 
    {
        return "MiauEvent";
    }

    public override void GetDataCallback()
    {
        throw new System.NotImplementedException();
    }
}