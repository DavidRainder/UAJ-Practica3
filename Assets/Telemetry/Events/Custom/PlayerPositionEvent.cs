using System.Xml;
using UnityEngine;

public class PlayerPositionEvent : TelemetrySystem.TrackerPersistentEvent
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
            $", \"position\": {{\"x\":\"{position.x}\",\"y\":\"{position.y}\"}}";
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