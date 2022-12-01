using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PHPTestPSQL : MonoBehaviour
{
    private string _urlRoot = "https://museumistu.epizy.com/PHP/";    
    
    private void Start()
    {
        //StartCoroutine(GetRequest(_urlRoot + "database.php"));
        //StartCoroutine(LoginQuantityEnumerator(_urlRoot + "loginq.php"));
        //StartCoroutine(Calc(_urlRoot + "calc.php"));
        //StartCoroutine(Calc2(_urlRoot + "calc.php"));
        //StartCoroutine(Calc3(_urlRoot + "calc.php"));
        StartCoroutine(TestGet(_urlRoot + "test_get.php"));
        //StartCoroutine(Calc2(_urlRoot + "test_get.php"));
        //StartCoroutine(GetTextFromWWW(_urlRoot + "test_get.php"));
        
        //Works
        //StartCoroutine(GetTextFromWWW("https://google.com/"));
        //Empty
        //StartCoroutine(GetTextFromWWW("https://museumistu.epizy.com/"));
    }
    
    private IEnumerator GetTextFromWWW (string url)
    {
        WWW www = new WWW(url);
         
        yield return www;
         
        if (www.error != null)
        {
            Debug.Log("Ooops, something went wrong...");
        }
        else
        {
            Debug.Log(www.text);
        }
    }
    private IEnumerator GetRequest(string url)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
    
    private IEnumerator TestGet(string url)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator Calc3(string url)
    {
        yield return new WaitForSecondsRealtime(2f);
        WWWForm form = new WWWForm();
        form.AddField ("ValueA", "22");
        form.AddField ("ValueB", "44");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            
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
    private IEnumerator Calc2(string url)
    {
        CustomCertificateHandler  certHandler = new CustomCertificateHandler();
        var uploadHandler = new UploadHandlerRaw(new byte[]{});
        var downloadHandler = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(url, "GET", downloadHandler, uploadHandler);
        www.url = url;
        www.certificateHandler = certHandler;
        {
            yield return www.SendWebRequest();
            Debug.Log(www.url);
            Debug.Log(downloadHandler.text);
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
    
    private IEnumerator Calc(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField ("ValueA", "22");
        form.AddField ("ValueB", "44");
        CustomCertificateHandler  certHandler = new CustomCertificateHandler();
        var uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(form.ToString()));
        var downloadHandler = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(url, "POST", downloadHandler, uploadHandler);
        www.url = url;
        www.certificateHandler = certHandler;
        {
            yield return www.SendWebRequest();
            Debug.Log(www.url);
            Debug.Log(downloadHandler.text);
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

            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler.text);
            else
            {
                string responseText = www.downloadHandler.text;

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
