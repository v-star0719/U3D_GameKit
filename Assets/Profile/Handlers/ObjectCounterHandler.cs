using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Linq;


namespace CamelGames.Tools.Profile.Handlers
{
    public class ObjectCounterHandler : IProfileHandler
    {
        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var snapshot = ProfileServer.ActiveServer.ObjectCounter.GenerateSnapshot();
            var json = "{ " + string.Join(",", snapshot.Select(e => "\"" + e.Key.Name + "\":" + e.Value).ToArray()) + " }";
            var bodyBytes = Encoding.UTF8.GetBytes(json);
            response.ContentType = "text/json";
            response.ContentLength64 = bodyBytes.LongLength;
            response.OutputStream.Write(bodyBytes, 0, bodyBytes.Length);
        }
    }
}
