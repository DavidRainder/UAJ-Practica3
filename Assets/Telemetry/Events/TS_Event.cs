
using System;
using System.Numerics;
using System.Security.Authentication.ExtendedProtection;
using TelemetrySystem;
using UnityEngine;

namespace TelemetrySystem
{
    public abstract class Event
    {
        protected long _timeStamp;
        public long TimeStamp { get { return _timeStamp; } }

        public Event(DateTimeOffset time)
        {
            _timeStamp = time.ToUnixTimeMilliseconds();
        }
        public abstract string GetID();

        public virtual string ToJSON()
        {
            return $"\"event_type\": \"{GetID()}\", \"time_stamp\": \"{TimeStamp.ToString()}\"";
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

        public PersistentEvent(int persistencyTime) : base(DateTimeOffset.UtcNow)
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
            _timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public long AdvanceTimer()
        {
            _currentPersistentTime += PersistentTime;
            return _currentPersistentTime;
        }
    }
}

#region SYSTEM_EVENTS
public class GameStartEvent : TelemetrySystem.Event
{
    public GameStartEvent() : base(DateTimeOffset.UtcNow) { }
    public override string GetID() => "ApplicationStart";
}

public class GameEndEvent : TelemetrySystem.Event
{
    public GameEndEvent() : base(DateTimeOffset.UtcNow) { }
    public override string GetID() => "ApplicationEnd";
}

public class LevelStartEvent : TelemetrySystem.Event
{
    public string levelName; 
    public LevelStartEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string GetID() => "LevelStart";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }
}

public class LevelEndEvent : TelemetrySystem.Event
{
    public string levelName;
    public LevelEndEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string GetID() => "LevelEnd";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }
}
public class LevelPauseEvent : TelemetrySystem.Event
{
    public string levelName;
    public LevelPauseEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string GetID() => "LevelPause";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }
}
public class LevelUnpauseEvent : TelemetrySystem.Event
{
    public string levelName;
    public LevelUnpauseEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string GetID() => "LevelUnpause";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }
}
public class LevelRestartEvent : TelemetrySystem.Event
{
    public string levelName;
    public LevelRestartEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string GetID() => "LevelRestart";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }
}
#endregion

#region PROPIOS
public class PlayerDeathEvent : TelemetrySystem.Event
{
    public UnityEngine.Vector2 position;
    public PlayerDeathEvent(UnityEngine.Vector2 pos) : base(DateTimeOffset.UtcNow)
    {
        position = pos;
    }
    public override string GetID() => "PlayerDeath";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"death_position\": {{\"x\":{position.x},\"y\":{position.y}}}";
    }
}
public class InteractionEvent : TelemetrySystem.Event
{
    public string objectName;
    public bool success;
    public InteractionEvent(string obj, bool correct) : base(DateTimeOffset.UtcNow)
    {
        objectName = obj;
        success = correct;
    }
    public override string GetID() => "Interaction";
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"object\": \":{objectName}\", \"success\":{success}";
    }
}

public class PlayerPositionEvent : TelemetrySystem.PersistentEvent
{
    public UnityEngine.Vector2 position;
    private Transform playerTransform;
    public PlayerPositionEvent(Transform player, int persistencyTime) : base(persistencyTime)
    {
        playerTransform = player;
    }
    public override string GetID() => "PlayerPosition";

    public override void GetDataCallback()
    {
        position = playerTransform.position;
    }
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"position\": {{\"x\":{position.x},\"y\":{position.y}}}";
    }
}
#endregion
