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
    public int floor;
}

public class DriveGet : MonoBehaviour
{
    [SerializeField] private GameObject _hallPrefab;
    [SerializeField] private Painting _paintingPrefab; 
    private GameObject _parent;
    public List<HallElement> hallElements;
    public string tableName = "Hall1";
    private RectTransform _contentRT;
    
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

        RefreshHall();
        Drive.responseCallback -= HandleDriveResponse;
    }

    public void Setup()
    {
        _parent = Instantiate(_hallPrefab, Vector3.zero, Quaternion.identity, GameObject.FindWithTag("Canvas").transform);
        _contentRT = _parent.transform.GetChild(0).GetComponent<RectTransform>();
        Drive.responseCallback += HandleDriveResponse;
        Drive.GetTable(tableName, true);
    }

    private void RefreshHall()
    {
        for (int i = 0; i < hallElements.Count; i++)
        {
            Painting painting = Instantiate(_paintingPrefab, Vector3.zero, Quaternion.identity, _parent.transform.GetChild(0));
            painting.StartCoroutine(painting.LoadImage(hallElements[i].image_url));
            //painting.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(i / 2 * 250f), 300f * (i % 2));
        }
        _contentRT.anchoredPosition = new Vector2(0f, 150f);
    }
}
