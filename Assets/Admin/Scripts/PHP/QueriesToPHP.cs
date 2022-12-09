using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class QueriesToPHP
{
    private string _urlRoot = "https://istu-museum-admin.netlify.app/api/PHP/";    

    public IEnumerator GetRequest(string phpFileName, Action<string> responseCallback)
    {
        string fullUrl = _urlRoot + phpFileName;
        using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
        {
            yield return www.SendWebRequest();
            Debug.Log($"{phpFileName} GET request");
            
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log($"Url: {www.uri} | Error: {www.error} | {www.downloadHandler?.text}");
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
            Debug.Log($"{phpFileName} POST request");
            
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log($"Url: {www.uri} | Error: {www.error} | {www.downloadHandler?.text}");
            else
                responseCallback?.Invoke(www.downloadHandler.text);
        }
    }
}
