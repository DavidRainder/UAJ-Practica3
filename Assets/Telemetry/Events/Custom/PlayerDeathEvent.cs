using System;
using System.Xml;

public class PlayerDeathEvent : TelemetrySystem.TrackerEvent
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
            $", \"death_position\": {{\"x\":\"{position.x}\",\"y\":\"{position.y}\"}}";
    }

    public override string ToXML(XmlDocument doc, XmlNode eventsNode, out XmlNode myEvent)
    {
        base.ToXML(doc, eventsNode, out myEvent);

        XmlAttribute playerPosition = doc.CreateAttribute("player_position");
        playerPosition.Value = $"X: {position.x}, Y: {position.y}";
        myEvent.Attributes.Append(playerPosition);
        return myEvent.OuterXml;

    }
}
