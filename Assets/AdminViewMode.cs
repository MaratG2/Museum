using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminViewMode : MonoBehaviour
{
    [SerializeField] private GameObject _hallPreview;
    [SerializeField] private GameObject _textGORefreshing;
    [SerializeField] private Button _hallListingPrefab;
    [SerializeField] private RectTransform _hallListingsParent;

    private string _tableOptionsName = "Options";
    private AdminNewMode.HallOptions _hallSelected;
    private List<AdminNewMode.HallOptions> _cachedHallOptions;

    public AdminNewMode.HallOptions HallSelected
    {
        get => _hallSelected;
        set => _hallSelected = value;
    }

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

    public void SelectHall(int num)
    {
        if (Convert.ToBoolean(_cachedHallOptions[num].is_deleted))
            return;
        
        Debug.Log(num + " | " + _cachedHallOptions.Count);
        if(_hallSelected.name != _cachedHallOptions[num].name)
            FindObjectOfType<AdminEditMode>().ClearAll();
        
        _hallSelected = _cachedHallOptions[num];
    }

    public void GoToWebInterface()
    {
        Application.OpenURL("https://docs.google.com/spreadsheets/d/1cjU08lg0u6w_ys3M87C6UCgx8mWUjaUEwwSOsDuXm1k/edit#gid=756982139");
    }
    
    public void Refresh()
    {
        for (int i = 0; i < _hallListingsParent.childCount; i++)
            Destroy(_hallListingsParent.GetChild(i).gameObject);
        _textGORefreshing.SetActive(true);
        _hallPreview.SetActive(false);
        Invoke(nameof(DelayRefresh), 0.5f);
    }

    private void DelayRefresh()
    {
        Drive.GetTable(_tableOptionsName, true);
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        Debug.Log(dataContainer.msg);
        _textGORefreshing.SetActive(false);
        _hallPreview.SetActive(true);
        if (dataContainer.QueryType == Drive.QueryType.getTable)
        {
            string rawJSon = dataContainer.payload;
            Debug.Log(rawJSon);

            // Check if the type is correct.
            if (string.Compare(dataContainer.objType, _tableOptionsName) == 0)
            {
                // Parse from json to the desired object type.
                AdminNewMode.HallOptions[] options = JsonHelper.ArrayFromJson<AdminNewMode.HallOptions>(rawJSon);
                _cachedHallOptions = options.ToList();
                string logMsg = "<color=yellow>" + options.Length.ToString() + " hall options retrieved from the cloud and parsed:</color>";
                for (int i = 0; i < options.Length; i++)
                {
                    if (Convert.ToBoolean(options[i].is_deleted))
                        continue;
                    var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                        _hallListingsParent);
                    newInstance.gameObject.name = i.ToString();
                    newInstance.GetComponentInChildren<TextMeshProUGUI>().text = options[i].name;
                    newInstance.onClick.AddListener(() => SelectHall(Int32.Parse(newInstance.gameObject.name)));
                }
            }
        }
    }
}