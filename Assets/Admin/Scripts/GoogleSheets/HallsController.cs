using System;
using GoogleSheetsForUnity;
using UnityEngine;

public class HallsController : MonoBehaviour
{
    [SerializeField] private DriveGet _hallPrefab;
    
    private void OnEnable()
    {
        // Suscribe for catching cloud responses.
        Drive.responseCallback += HandleDriveResponse;
    }

    private void OnDisable()
    {
        // Remove listeners.
        Drive.responseCallback -= HandleDriveResponse;
    }
    
    void Awake()
    {
        Drive.GetAllTables(true);
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        if (dataContainer.QueryType == Drive.QueryType.getAllTables)
        {
            string rawJSon = dataContainer.payload;

            // The response for this query is a json list of objects that hold tow fields:
            // * objType: the table name (we use for identifying the type).
            // * payload: the contents of the table in json format.
            Drive.DataContainer[] tables = JsonHelper.ArrayFromJson<Drive.DataContainer>(rawJSon);

            // Once we get the list of tables, we could use the objTypes to know the type and convert json to specific objects.
            // On this example, we will just dump all content to the console, sorted by table name.
            string logMsg = "<color=yellow>All data tables retrieved from the cloud.\n</color>";
            for (int i = 0; i < tables.Length; i++)
            {
                string tableName = tables[i].objType;
                var hall = Instantiate(_hallPrefab, Vector3.zero, Quaternion.identity);
                hall.gameObject.name = tableName;
                hall.tableName = tableName;
                hall.Setup();
                logMsg += "\n<color=blue>Table Name: " + tables[i].objType + "</color>\n" + tables[i].payload + "\n";
            }

            Debug.Log(logMsg);
        }
    }
}
