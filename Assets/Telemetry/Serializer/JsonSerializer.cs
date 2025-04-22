namespace TelemetrySystem
{
    public class JsonSerializer : ISerializer
    {
        bool firstEvent = true;

        public string StartingContent()
        {
            return "{\n\"events\": [\n";
        }

        public string FinalContent()
        {
            return "]\n}";
        }

        public string Serialize(TrackerEvent e)
        {
            string content = "";

            if (firstEvent)
            {
                content += "{";
                firstEvent = false;
            }
            else content += ",{";

            content += e.ToJSON();
            content += "}\n";

            return content;
        }
    }
}