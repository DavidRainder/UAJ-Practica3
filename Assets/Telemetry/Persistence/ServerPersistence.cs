using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TelemetrySystem
{
    public class ServerPersistence : IPersistence
    {
        string serverPath; //Este path es de ejemplo, habria que cambiarlo al link del servidor
        ISerializer serializer;

        public ServerPersistence(string _serverPath, ISerializer serializer)
        {
            serverPath = _serverPath;
            this.serializer = serializer;
        }

        public void EndFlush()
        {
            // No es necesario implementar al no tener que acabar ningún archivo
            // Posiblemente este método se encargaría de desconcetarse del servidor, si ese fuera el acercamiento planteado.
        }

        public void Flush(ref Queue<TrackerEvent> eQueue)
        {
            string content = "";
            while (eQueue.Count > 0)
            {
                content += serializer.Serialize(eQueue.Dequeue());
            }

            Upload(content);
        }

       
        void Upload(string content)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serverPath);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            //Envío de trazas al servidor
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = content;

                streamWriter.Write(json);
                streamWriter.Close();
            }

            ////Respuesta del servidor --> Debería haber un error handler, ejemplo básico de nuestro sistema de persistencia el envío de 
            ///trazas al servidor. Al no haber servidor no se puede probar apropiadamente
            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //}
        }
    }
}