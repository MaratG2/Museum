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
    
    private HallOptions _hallSelected;
    private List<HallOptions> _cachedHallOptions = new List<HallOptions>();
    private Vector2 _startTilePos;
    
    public static NpgsqlConnection dbcon;
    
    public HallOptions HallSelected
    {
        get => _hallSelected;
        set => _hallSelected = value;
    }

    private void Start()
    { 
        string connectionString =
            "Port = 5432;" +
            "Server= ec2-54-77-40-202.eu-west-1.compute.amazonaws.com;" +
            "Database= dp3oh4vja8l35;" +
            "User ID= eudqcffpovolpi;" +
            "Password= 65f254f251471be22f035c26958c8cfad49fc31c9e8134febf4f4c165bd47665;" +
            "sslmode=Prefer;" +
            "Trust Server Certificate=true";
        dbcon = new NpgsqlConnection(connectionString);
        dbcon.Open();
        Refresh();
    }

    public void SelectHall(int num)
    {
        HallOptions currentOption = new HallOptions();
        bool hasFound = false;
        foreach (var cho in _cachedHallOptions)
        {
            if (cho.onum == num)
            {
                currentOption = cho;
                hasFound = true;
            }
        }
        if (!hasFound)
        {
            Debug.LogError("NOT FOUND OPTION BY THAT ONUM");
            return;
        }
        
        if (_hallSelected.name != currentOption.name)
        {
            FindObjectOfType<AdminEditMode>().ClearAll();
            for (int i = 0; i < _tilesParent.transform.childCount; i++)
                Destroy(_tilesParent.transform.GetChild(i).gameObject);
        }
        _hallSelected = currentOption;
        StartCoroutine(FindLeftBottomTile(num));
    }

    public void GoToWebInterface()
    {
        Application.OpenURL("https://docs.google.com/spreadsheets/d/1cjU08lg0u6w_ys3M87C6UCgx8mWUjaUEwwSOsDuXm1k/edit#gid=756982139");
    }
    
    private void Paint(Vector2 tiledPos, Vector2 pos, HallContent content)
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
        HallSelected = new HallOptions();
        for (int i = 0; i < _tilesParent.transform.childCount; i++)
            Destroy(_tilesParent.transform.GetChild(i).gameObject);
        _textGORefreshing.SetActive(true);
        _hallPreview.SetActive(false);
        SQLGetAllOptions();
    }

    private void SQLGetAllOptions()
    {
        _cachedHallOptions = new List<HallOptions>();
        
        NpgsqlCommand dbcmd = dbcon.CreateCommand();
        string sql =
            "SELECT * FROM " +
            "options_view";
        dbcmd.CommandText = sql;
        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int onum = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
            string name = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1);
            int sizex = (reader.IsDBNull(2)) ? 0 : reader.GetInt32(2);
            int sizez = (reader.IsDBNull(3)) ? 0 : reader.GetInt32(3);
            bool is_date_b = reader.GetBoolean(4);
            bool is_date_e = reader.GetBoolean(5);
            string date_begin = (reader.IsDBNull(6)) ? "NULL" : reader.GetDateTime(6).ToShortDateString();
            string date_end = (reader.IsDBNull(7)) ? "NULL" : reader.GetDateTime(7).ToShortDateString();
            bool is_maintained = reader.GetBoolean(8);
            bool is_hidden = reader.GetBoolean(9);
            
            HallOptions newOption = new HallOptions();
            newOption.onum = onum;
            newOption.name = name;
            newOption.sizex = sizex;
            newOption.sizez = sizez;
            newOption.is_date_b = is_date_b;
            newOption.is_date_e = is_date_e;
            newOption.date_begin = date_begin;
            newOption.date_end = date_end;
            newOption.is_maintained = is_maintained;
            newOption.is_hidden = is_hidden;
            _cachedHallOptions.Add(newOption);
            
            var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                _hallListingsParent);
            newInstance.gameObject.name = (onum).ToString();
            newInstance.GetComponentInChildren<TextMeshProUGUI>().text = name;
            newInstance.onClick.AddListener(() => SelectHall(onum));
        }
        
        reader.Close();
        reader = null;
        
        _textGORefreshing.SetActive(false);
        _hallPreview.SetActive(true);
    }

    private void SQLGetContentByOnum(int num)
    {
        NpgsqlCommand dbcmd = dbcon.CreateCommand();
        string sql =
            "SELECT c.cnum, c.title, c.image_desc, c.image_url, c.pos_x, c.pos_z, c.combined_pos, c.type " +
            "FROM public.options AS o " +
            "JOIN public.contents AS c ON " + num + " = c.onum";
        dbcmd.CommandText = sql;
        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
        while (reader.Read())
        {
            HallContent content = new HallContent();
            int onum = num;
            int cnum = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
            string title = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1);
            string image_desc = (reader.IsDBNull(2)) ? "NULL" : reader.GetString(2);
            string image_url = (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3);
            int pos_x = (reader.IsDBNull(4)) ? 0 : reader.GetInt32(4);
            int pos_z = (reader.IsDBNull(5)) ? 0 : reader.GetInt32(5);
            string combined_pos = (reader.IsDBNull(6)) ? "NULL" : reader.GetString(6);
            int type = (reader.IsDBNull(7)) ? 0 : reader.GetInt32(7);

            content.onum = onum;
            content.cnum = cnum;
            content.title = title;
            content.image_desc = image_desc;
            content.image_url = image_url;
            content.pos_x = pos_x;
            content.pos_z = pos_z;
            content.combined_pos = combined_pos;
            content.type = type;
            
            Vector2 tilePos = _startTilePos + new Vector2(content.pos_x, content.pos_z);
            Vector2 drawPos = new Vector2
            (
                tilePos.x * tileSize,
                tilePos.y * tileSize
            );
            Paint(tilePos, drawPos, content);
        }
        
        reader.Close();
        reader = null;
    }
    
    private IEnumerator FindLeftBottomTile(int num)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        float tileSize = 0f;
        while (tileSize.Equals(0f))
        {
            tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
            yield return new WaitForEndOfFrame();
        }
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
                    SQLGetContentByOnum(num);
                    yield break;
                }
            }
        }
    }
}
