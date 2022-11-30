using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PHPTestPSQL : MonoBehaviour
{
    private string _urlRoot = "http://istu-museum-admin.netlify.app/PHP/";    
    
    private void Start()
    {
        StartCoroutine(GetRequest(_urlRoot + "database.php"));
        //StartCoroutine(LoginQuantityEnumerator(_urlRoot + "loginq.php"));
    }
    
    private IEnumerator GetRequest(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        print(System.Text.Encoding.Default.GetString(www.bytes));
        print(www.text);

        /*
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log("In Using");
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) 
                Debug.Log("Url: " + www.uri + " | Error: " + www.error);
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
        */
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
