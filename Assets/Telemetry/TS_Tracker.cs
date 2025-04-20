using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System;
using System.Xml;

namespace TelemetrySystem {

    public class Tracker : MonoBehaviour
    {
        #region Singleton
        private static Tracker _instance = null;
        public static Tracker Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        enum SerializationFormat { JSON, XML }

        #region Parameters
        [SerializeField] float _timeToDumpQueue;
        [SerializeField] string _fileDestinationName = "Telemetry";
        [SerializeField] SerializationFormat _outputFormat = SerializationFormat.JSON;
        #endregion

        private void Start()
        {
            _events = new Queue<Event>();
            _persistentEvents = new PriorityQueue<PersistentEvent, long>();
            _eventRegistry = GetComponent<EventRegistry>();

            // error handling de ^^

            mutEvents = new Mutex();
            mutPersistentEvents = new Mutex();

            Parallel.Invoke(DumpEvents, PersistentEventTracking);

            PushEvent(new InteractionEvent("miau", true));
            TrackPersistentEvent(new PlayerPositionEvent(this.transform, 100));
        }

        private void OnDestroy()
        {
            destroyed = true;
        }

        #region Private Variables
        private Mutex mutEvents;
        private Mutex mutPersistentEvents;

        static bool destroyed = false;

        EventRegistry _eventRegistry = null;

        Queue<Event> _events;
        // el Long es el tiempo en POSIX en el que se debe ejecutar
        PriorityQueue<PersistentEvent, long> _persistentEvents;

        int numSession = 0;
        string baseFileName = "";
        string finalFileName = "";

        // For XML Serialization
        XmlDocument xmlDocument = null;
        XmlNode eventsNode = null;
        #endregion

        private void CheckPreviousFiles(string fileExtension)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/");

            numSession = 0;
            baseFileName = _fileDestinationName + DateTime.Now.ToString("-d-M-yyyy") + fileExtension;
            Debug.Log("Files under the destiny directory: ");

            foreach (FileInfo info in directoryInfo.GetFiles())
            {
                string f = info.Name.Split("_")[1];
                if (f == baseFileName)
                {
                    numSession++;
                }
            }

            finalFileName = Application.persistentDataPath + "/" + numSession + "_" + baseFileName;
        }

        private void OpenAndStartXMLFile()
        {
            CheckPreviousFiles(".xml");

            xmlDocument = new XmlDocument();
            eventsNode = xmlDocument.CreateElement("events");
            xmlDocument.AppendChild(eventsNode);
        }

        private void GetXMLContentFromEvent(Event e)
        {
            e.ToXML(xmlDocument, eventsNode, out XmlNode myEvent);
        }

        private void CloseAndEndXMLFile()
        {
            if (xmlDocument != null) {
                xmlDocument.Save(finalFileName);
            }
            else
            {
                // error handling
            }
        }

        private void OpenAndStartJSONFile()
        {
            CheckPreviousFiles(".json");

            FileStream fileFirst = File.Open(
                    finalFileName,
                    FileMode.Append);

            fileFirst.Write(new UTF8Encoding(true).GetBytes("{\n\"events\": [\n"));
            fileFirst.Close();
        }

        bool firstEvent = true;
        private string GetJSONContentFromEvent(Event e)
        {
            string content = "";

            if (firstEvent)
            {
                content += "{\n";
                firstEvent = false;
            }
            else content += ",{\n";

            content += e.ToJSON();
            content += "\n}\n";

            return content;
        }

        private void CloseAndEndJSONFile()
        {
            FileStream fileLast = File.Open(
                    finalFileName,
                    FileMode.Append);

            fileLast.Write(new UTF8Encoding(true).GetBytes("\n]\n}"));
            fileLast.Close();
        }

        private void WriteToFile(string content)
        {
            var encodedContent = new UTF8Encoding(true).GetBytes(content);

            FileStream file = File.Open(
                finalFileName,
                FileMode.Append);

            file.Write(encodedContent);

            file.Close();
        }

        async void DumpEvents()
        {
            switch (_outputFormat)
            {
                case SerializationFormat.JSON:
                    OpenAndStartJSONFile();
                    break;
                case SerializationFormat.XML:
                    OpenAndStartXMLFile();
                    break;
            }

            while (!destroyed)
            {
                await Task.Delay((int)(_timeToDumpQueue * 1000));
                
                mutEvents.WaitOne();

                switch (_outputFormat)
                {
                    case SerializationFormat.JSON:

                        string content = "";
                        while (_events.Count > 0)
                        {
                            Event e = _events.Dequeue();
                            content += GetJSONContentFromEvent(e);
                        }

                        WriteToFile(content);

                        break;
                    case SerializationFormat.XML:

                        while (_events.Count > 0)
                        {
                            GetXMLContentFromEvent(_events.Dequeue());
                        }

                        break;
                }

                mutEvents.ReleaseMutex();

                Debug.Log("Events dumped");
            }

            switch (_outputFormat)
            {
                case SerializationFormat.JSON:
                    CloseAndEndJSONFile();
                    break;
                case SerializationFormat.XML:
                    CloseAndEndXMLFile();
                    break;
            }
        }

        async void PersistentEventTracking()
        {
            bool empty = !_persistentEvents.TryPeek(out PersistentEvent _, out long firstPrio);
            
            if (empty)
                return;
            
            long currentTimeStamp = firstPrio;
            while(!destroyed)
            {
                // mutex
                mutPersistentEvents.WaitOne();

                // pillas un evento
                _persistentEvents.TryDequeue(out PersistentEvent e, out long priority);
                
                // unlock
                mutPersistentEvents.ReleaseMutex();

                // Debug.Log("Waiting for: " + (priority - currentTimeStamp).ToString() + "ms.");
                
                await Task.Run(async () => { await Task.Delay((int)(priority - currentTimeStamp)); });

                currentTimeStamp = priority;

                e.UpdateTimeStamp();

                e.GetDataCallback();

                // mutex
                mutEvents.WaitOne();

                PushEvent(e);
                
                // unlock
                mutEvents.ReleaseMutex();

                // mutex
                mutPersistentEvents.WaitOne();
                
                _persistentEvents.Enqueue(e, e.AdvanceTimer());
                
                // unlock
                mutPersistentEvents.ReleaseMutex();
            }
        }


        public void PushEvent(Event e)
        {
            // Si el evento está activo...
            if (!_eventRegistry.IsEventActive(e.GetID())) return;

            // LOCK MUTEX
            mutEvents.WaitOne();
            
            _events.Enqueue(e);

            // UNLOCK MUTEX
            mutEvents.ReleaseMutex();

            Debug.Log("Event Pushed");
        }

        public void TrackPersistentEvent(PersistentEvent e)
        {
            // Si está activo... 
            if (!_eventRegistry.IsEventActive(e.GetID())) return;

            e.UpdatePersistentTime();

            // lo metemos en la cola
            // LOCK MUTEX
            mutPersistentEvents.WaitOne();

            bool wasEmpty = _persistentEvents.Count == 0;

            _persistentEvents.Enqueue(e, e._currentPersistentTime);

            // UNLOCK MUTEX
            mutPersistentEvents.ReleaseMutex();

            if(wasEmpty)
            {
                Parallel.Invoke(PersistentEventTracking);
            }
        }
    }
}
