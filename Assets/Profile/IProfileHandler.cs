using UnityEngine;
using System.Collections;
using System.Net;


namespace CamelGames.Tools.Profile
{
    public interface IProfileHandler
    {
        void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
    }
}
