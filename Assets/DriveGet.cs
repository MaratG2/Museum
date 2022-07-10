using System;
using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
using UnityEngine;

[Serializable]
public class HallElement
{
    public string title;
    public string image_url;
    public string image_desc;
    public int id;
}

public class DriveGet : MonoBehaviour
{
    public List<HallElement> hallElements;
    public string tableName = "Hall1";
    
    // Overwrites local translation data with the table obtained from the cloud.
    [ContextMenu("Download Localization Table")]
    private void RetrieveCloudData()
    {
        // Suscribe for catching cloud responses.
        Drive.responseCallback += HandleDriveResponse;
        // Make the query.
        Drive.GetTable(tableName, false);
    }

    [ContextMenu("Upload Localization Table")]
    private void AddAllKeysToTable()
    {
        // Suscribe to Drive event to get the Drive response.
        Drive.responseCallback += HandleDriveResponse;
                        
        string jsonData = JsonHelper.ToJson(hallElements.ToArray());
        Drive.CreateObjects(jsonData, tableName, false);
    }
    
    private void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        if (dataContainer.objType != tableName)
            return;

        // First check the type of answer.
        if (dataContainer.QueryType == Drive.QueryType.getTable)
        {
            string rawJSon = dataContainer.payload;
            Debug.Log("Data from Google Drive received.");

            // Parse from json to the desired object type.
            HallElement[] elements = JsonHelper.ArrayFromJson<HallElement>(rawJSon);
            hallElements = new List<HallElement>(elements);
        }

        if (dataContainer.QueryType != Drive.QueryType.createTable || dataContainer.QueryType != Drive.QueryType.createObjects)
        {
            Debug.Log(dataContainer.msg);
        }

        Drive.responseCallback -= HandleDriveResponse;
    }
}
