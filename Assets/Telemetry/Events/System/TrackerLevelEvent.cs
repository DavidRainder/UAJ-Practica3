using System;
using System.Xml;

public abstract class TrackerLevelEvent : TelemetrySystem.TrackerEvent
{
    public string levelName;
    public TrackerLevelEvent(string _levelName) : base(DateTimeOffset.UtcNow)
    {
        levelName = _levelName;
    }
    public override string ToJSON()
    {
        return base.ToJSON() +
            $", \"level_name\": \"{levelName}\"";
    }

    public override string ToXML(XmlDocument doc, XmlNode eventsNode, out XmlNode myEvent)
    {
        base.ToXML(doc, eventsNode, out myEvent);

        XmlAttribute levelName = doc.CreateAttribute("level_name");
        levelName.Value = this.levelName;
        myEvent.Attributes.Append(levelName);
        return myEvent.OuterXml;

    }
}
