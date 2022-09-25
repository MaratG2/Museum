using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminViewMode : MonoBehaviour
{
    [SerializeField] private Button _hallListingPrefab;
    [SerializeField] private RectTransform _hallListingsParent;

    private string _tableOptionsName = "Options";

    private void OnEnable()
    {
        // Suscribe for catching cloud responses.
        Drive.responseCallback += HandleDriveResponse;
        Refresh();
    }

    private void OnDisable()
    {
        // Remove listeners.
        Drive.responseCallback -= HandleDriveResponse;
    }

    public void Refresh()
    {
        for (int i = 0; i < _hallListingsParent.childCount; i++)
            Destroy(_hallListingsParent.GetChild(i).gameObject);
        
        Invoke(nameof(DelayRefresh), 0.5f);
    }

    private void DelayRefresh()
    {
        Drive.GetTable(_tableOptionsName, true);
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        Debug.Log(dataContainer.msg);

        if (dataContainer.QueryType == Drive.QueryType.getTable)
        {
            string rawJSon = dataContainer.payload;
            Debug.Log(rawJSon);

            // Check if the type is correct.
            if (string.Compare(dataContainer.objType, _tableOptionsName) == 0)
            {
                // Parse from json to the desired object type.
                AdminNewMode.HallOptions[] options = JsonHelper.ArrayFromJson<AdminNewMode.HallOptions>(rawJSon);
                string logMsg = "<color=yellow>" + options.Length.ToString() + " hall options retrieved from the cloud and parsed:</color>";
                for (int i = 0; i < options.Length; i++)
                {
                    var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                        _hallListingsParent);
                    newInstance.GetComponentInChildren<TextMeshProUGUI>().text = options[i].name;
                }
            }
        }
    }
}
