using System;
using System.Xml;
using UnityEngine;

public class InteractionEvent : TelemetrySystem.TrackerEvent
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
            $", \"object\": \"{objectName}\", \"success\":\"{success}\"";
    }

    public override string ToXML(XmlDocument doc, XmlNode eventsNode, out XmlNode myEvent)
    {
        base.ToXML(doc, eventsNode, out myEvent);

        XmlAttribute myObject = doc.CreateAttribute("object_name");
        myObject.Value = objectName;
        myEvent.Attributes.Append(myObject);

        XmlAttribute mySuccess = doc.CreateAttribute("success");
        mySuccess.Value = success.ToString();
        myEvent.Attributes.Append(mySuccess);
        return myEvent.OuterXml;

    }
}
