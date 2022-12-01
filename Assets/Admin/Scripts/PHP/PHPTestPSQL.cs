using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PHPTestPSQL : MonoBehaviour
{
    //wokring, but no php ini edit - http://museumistu.atwebpages.com/PHP/
    private string _urlRoot = "http://museumistu.atwebpages.com/PHP/";    
    
    private void Start()
    {
        //StartCoroutine(GetRequest(_urlRoot + "database.php"));
        StartCoroutine(GetRequest(_urlRoot + "database2.php"));
        StartCoroutine(GetRequest(_urlRoot + "test_get.php"));
        StartCoroutine(LoginQuantityEnumerator(_urlRoot + "loginq.php"));
        StartCoroutine(Calc(_urlRoot + "calc.php"));
    }
    private IEnumerator GetRequest(string url)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            Debug.Log($"GET request ({url}): ");
            
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
    private IEnumerator Calc(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField ("ValueA", "22");
        form.AddField ("ValueB", "44");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            Debug.Log($"CALC request ({url}): ");
            
            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response: " + responseText);
                if (responseText.StartsWith("Success"))
                {
                    string[] dataChunks = responseText.Split('|');
                    int quantity = Int32.Parse(dataChunks[1]);
                    Debug.Log(quantity);
                }
                else
                {
                    Debug.Log("Error: responseText");
                }
            }
        }
    }
    private IEnumerator LoginQuantityEnumerator(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", "maratg2develop@gmail.com");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            Debug.Log($"LOGIN request ({url}): ");
            
            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler.text);
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log($"Login: {responseText}");
                if (responseText.StartsWith("Success"))
                {
                    string[] dataChunks = responseText.Split('|');
                    int quantity = Int32.Parse(dataChunks[1]);
                    Debug.Log(quantity);
                }
                else
                {
                    Debug.Log("Error: responseText");
                }
            }
        }
    }
}
