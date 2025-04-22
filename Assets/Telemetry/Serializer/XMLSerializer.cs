using System.Xml;

namespace TelemetrySystem
{
    public class XMLSerializer : ISerializer
    {
        XmlDocument xmlDocument = null;
        XmlNode eventsNode = null;

        public XMLSerializer()
        {
            xmlDocument = new XmlDocument();
            eventsNode = xmlDocument.CreateElement("events");
            xmlDocument.AppendChild(eventsNode);
        }

        public string Serialize(TrackerEvent e)
        {
            e.ToXML(xmlDocument, eventsNode, out XmlNode myEvent);
            return myEvent.OuterXml + "\n";
        }

        public string StartingContent()
        {
            return "<events>\n";
        }

        public string FinalContent()
        {
            return "</events>";
        }
    }
}