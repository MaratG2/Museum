using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class TimeManager
{
    private static DateTime _utcStartTime = DateTime.UtcNow;
    public static DateTime UtcNow => _utcStartTime.AddSeconds(Time.realtimeSinceStartup);
    public static bool IsReady = false;
    
    public static void InitializeTime()
    {
        GetUtcTimeAsync().WrapErrors();
    }
 
    private static async Task GetUtcTimeAsync()
    {
        try
        {
            /*
            string fullUrl = "http://time.nist.gov:13";
            UnityWebRequest www = UnityWebRequest.Get(fullUrl);
            UnityWebRequestAsyncOperation op = www.SendWebRequest();
            op.completed += (obj) =>
            {
                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Url: {www.uri} | Error: {www.error} | {www.downloadHandler?.text}");
                else
                {
                    var response = www.downloadHandler.data;
                    var responseString = response.ToString();
                    var utcDateTimeString = responseString.Substring(7, 17);
                    _utcStartTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    IsReady = true;
                }
            };
            */
            _utcStartTime = DateTime.Now;
        }
        catch
        {
            // Handle errors here
        }
    }
 
    private static async void WrapErrors(this Task task)
    {
        await task;
    }
}