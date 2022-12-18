using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Admin.PHP
{
    public class ClientPhpAsync
    {
        private readonly string baseRoute = "https://istu-museum-admin.netlify.app/api/PHP/";
        private bool _isDebugOn;

        public ClientPhpAsync(bool isDebugOn)
        {
            _isDebugOn = isDebugOn;
        }
        
        public async Task<string> PostRequestAsync(string phpFileName, WWWForm data)
        {
           
            var fullUrl = baseRoute + phpFileName;
            using UnityWebRequest request = UnityWebRequest.Post(fullUrl, data);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"Error while sending:{request.error}");
                return request.error;
            }

            var response = request.downloadHandler.text;
            return response;
        }
        
        public async Task<string> GetRequestAsync(string phpFileName)
        {
           
            var fullUrl = baseRoute + phpFileName;
            var request = UnityWebRequest.Get(fullUrl);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"Error while sending:{request.error}");
                return request.error;
            }

            var response = request.downloadHandler.text;
            return response;
        }
    }
}