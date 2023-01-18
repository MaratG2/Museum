using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Admin.PHP
{
    /// <summary>
    /// Отвечает за GET/POST запросы к PHP скриптам на сервере.
    /// </summary>
    /// <remarks>
    /// Запросы выполняются внутри IEnumerator методах (корутинах), которые принимают название PHP скрипта на сервере, возвращают – обратный вызов с текстом-ответом из скрипта.
    /// Для использования класса нужно сконструировать его C# объект, обращаться к методам через объект.
    /// </remarks>
    public class QueriesToPHP
    {
        private string _urlRoot = "https://istu-museum-admin.netlify.app/api/PHP/";
        private bool _isDebugOn;

        public QueriesToPHP(bool isDebugOn)
        {
            _isDebugOn = isDebugOn;
        }

        public IEnumerator GetRequest(string phpFileName, Action<string> responseCallback)
        {
            string fullUrl = _urlRoot + phpFileName;
            using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
            {
                yield return www.SendWebRequest();
                if (_isDebugOn)
                    Debug.Log($"{phpFileName} GET request");

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Url: {www.uri} | Error: {www.error} | {www.downloadHandler?.text}");
                else
                    responseCallback?.Invoke(www.downloadHandler.text);
            }
        }

        public IEnumerator PostRequest(string phpFileName, WWWForm data, Action<string> responseCallback)
        {
            string fullUrl = _urlRoot + phpFileName;
            using (UnityWebRequest www = UnityWebRequest.Post(fullUrl, data))
            {
                yield return www.SendWebRequest();
                if (_isDebugOn)
                    Debug.Log($"{phpFileName} POST request");

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Url: {www.uri} | Error: {www.error} | {www.downloadHandler?.text}");
                else
                    responseCallback?.Invoke(www.downloadHandler.text);
            }
        }
    }
}