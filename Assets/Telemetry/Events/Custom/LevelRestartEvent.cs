using UnityEngine;

public class LevelRestartEvent : TrackerLevelEvent
{
    private Vector2 position;

    public LevelRestartEvent(string _levelName, Vector2 pos) : base(_levelName) {
        position = pos;
    }

    public override string GetID() => "LevelRestart";

    public override string ToJSON()
    {
        return base.ToJSON()
            + $", \"position\": {{\"x\":\"{position.x}\",\"y\":\"{position.y}\"}}";
    }
}
