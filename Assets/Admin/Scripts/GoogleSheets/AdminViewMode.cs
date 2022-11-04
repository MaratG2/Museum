using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsForUnity;
using TMPro;
using Npgsql;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AdminViewMode : MonoBehaviour
{
    [SerializeField] private GameObject _hallPreview;
    [SerializeField] private GameObject _textGORefreshing;
    [SerializeField] private Button _hallListingPrefab;
    [SerializeField] private RectTransform _hallListingsParent;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _tilesParent;
    
    private string _tableOptionsName = "Options";
    private AdminNewMode.HallOptions _hallSelected;
    private List<AdminNewMode.HallOptions> _cachedHallOptions;
    private Vector2 _startTilePos;
    
    NpgsqlConnection dbcon;
    
    public AdminNewMode.HallOptions HallSelected
    {
        get => _hallSelected;
        set => _hallSelected = value;
    }

    private void Start()
    {
        string connectionString =
            "Port = 5432;"+
            "Server=localhost;" +
            "Database=museumistu;" +
            "User ID=postgres;" +
            "Password=postgres;";
        dbcon = new NpgsqlConnection(connectionString);
        dbcon.Open();
        Refresh();
    }

    public void SelectHall(int num)
    {
        if (Convert.ToBoolean(_cachedHallOptions[num].is_deleted))
            return;

        Debug.Log(num + " | " + _cachedHallOptions.Count);
        if (_hallSelected.name != _cachedHallOptions[num].name)
        {
            FindObjectOfType<AdminEditMode>().ClearAll();
            for (int i = 0; i < _tilesParent.transform.childCount; i++)
                Destroy(_tilesParent.transform.GetChild(i).gameObject);
        }
        _hallSelected = _cachedHallOptions[num];
        
        Invoke(nameof(FindLeftBottomTile), 0.5f);
    }

    public void GoToWebInterface()
    {
        Application.OpenURL("https://docs.google.com/spreadsheets/d/1cjU08lg0u6w_ys3M87C6UCgx8mWUjaUEwwSOsDuXm1k/edit#gid=756982139");
    }
    
    private void Paint(Vector2 tiledPos, Vector2 pos, AdminEditMode.HallContent content)
    {
        var newTile = Instantiate(_tilePrefab.gameObject, Vector2.zero, Quaternion.identity, _tilesParent.GetComponent<RectTransform>());
        newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchoredPosition = pos;
        newTile.GetComponent<Tile>().hallContent = content;
        newTile.GetComponent<Tile>().Setup();
        float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
        newTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
    }
    
    public void Refresh()
    {
        for (int i = 0; i < _hallListingsParent.childCount; i++)
            Destroy(_hallListingsParent.GetChild(i).gameObject);
        HallSelected = new AdminNewMode.HallOptions();
        for (int i = 0; i < _tilesParent.transform.childCount; i++)
            Destroy(_tilesParent.transform.GetChild(i).gameObject);
        _textGORefreshing.SetActive(true);
        _hallPreview.SetActive(false);
        Invoke(nameof(SQLGetAllOptions), 0.5f);
    }

    private void SQLGetAllOptions()
    {
        //Drive.GetTable(_tableOptionsName, true);
        NpgsqlCommand dbcmd = dbcon.CreateCommand();
        string sql =
            "SELECT * FROM " +
            "public.options";
        dbcmd.CommandText = sql;
        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string name = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1).ToString();
            int onum = (reader.IsDBNull(0)) ? 0 : Int32.Parse(reader.GetString(0));
            var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                _hallListingsParent);
            newInstance.gameObject.name = (onum).ToString();
            newInstance.GetComponentInChildren<TextMeshProUGUI>().text = name;
            newInstance.onClick.AddListener(() => SelectHall(onum));
        }
        
        reader.Close();
        reader = null;
    }
    
    private void FindLeftBottomTile()
    {
        float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;

        float addPosX = 0, addPosY = tileSize / 4;
        if(HallSelected.sizez % 2 == 0)
            addPosY += -tileSize / 4;
        if (HallSelected.sizex % 2 != 0)
            addPosX += tileSize / 2;

        for(int i = 0; i < 1920 / tileSize; i++)
        {
            for (int j = 0; j < 1080 / tileSize; j++)
            {
                bool isOverPreview = false;
                GameObject[] casted = AdminHallPreview.RaycastUtilities.UIRaycasts(
                    AdminHallPreview.RaycastUtilities.ScreenPosToPointerData(
                        new Vector2(i * tileSize + tileSize/2, j * tileSize + tileSize/4)));
                foreach (var c in casted)
                {
                    if (c.GetComponent<AdminHallPreview>())
                        isOverPreview = true;
                }

                if (isOverPreview)
                {
                    _startTilePos = new Vector2
                    (
                        i + 0.5f,
                        j + 0.25f
                    );
                    Drive.GetTable(HallSelected.name, true);
                    return;
                }
            }
        }
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

            if (string.Compare(dataContainer.objType, HallSelected.name) == 0)
            {
                AdminEditMode.HallContent[] players = JsonHelper.ArrayFromJson<AdminEditMode.HallContent>(rawJSon);
                float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
                for (int i = 0; i < players.Length; i++)
                {
                    Vector2 tilePos = _startTilePos + new Vector2(players[i].pos_x, players[i].pos_z);
                    Vector2 drawPos = new Vector2
                    (
                        tilePos.x * tileSize,
                        tilePos.y * tileSize
                    );
                    Paint(tilePos, drawPos, players[i]);
                    Debug.Log("IN");
                }
            }
            
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
