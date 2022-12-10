using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Button _modeSwitchEdit;
    [SerializeField] private Button _modeSwitchNew;
    private Action<string> _responseCallback;
    private QueriesToPHP _queriesToPhp = new (isDebugOn: true);
    private string _responseText;
    private Hall _hallSelected;
    private List<Hall> _cachedHalls = new List<Hall>();
    private Vector2 _startTilePos;

    public static NpgsqlConnection dbcon;

    public Hall HallSelected
    {
        get => _hallSelected;
        set => _hallSelected = value;
    }

    private void OnEnable()
    {
        _responseCallback += response => _responseText = response;
    }

    private void OnDisable()
    {
        _responseCallback -= response => _responseText = response;
    }

    private void Start()
    {
        _modeSwitchEdit.gameObject.SetActive(false);
        _modeSwitchNew.gameObject.SetActive(true);
        Refresh();
    }

    public void SelectHall(int num)
    {
        Hall current = new Hall();
        bool hasFound = false;
        foreach (var cho in _cachedHalls)
        {
            if (cho.hnum == num)
            {
                current = cho;
                hasFound = true;
            }
        }

        if (!hasFound)
        {
            Debug.LogError("NOT FOUND OPTION BY THAT ONUM");
            return;
        }

        if (_hallSelected.name != current.name)
        {
            FindObjectOfType<AdminEditMode>().ClearAll();
            for (int i = 0; i < _tilesParent.transform.childCount; i++)
                Destroy(_tilesParent.transform.GetChild(i).gameObject);
        }

        _hallSelected = current;
        _modeSwitchEdit.gameObject.SetActive(true);
        _modeSwitchNew.gameObject.SetActive(false);
        StartCoroutine(FindLeftBottomTile(num));
    }

    public void GoToWebInterface()
    {
        Application.OpenURL(
            "https://docs.google.com/spreadsheets/d/1cjU08lg0u6w_ys3M87C6UCgx8mWUjaUEwwSOsDuXm1k/edit#gid=756982139");
    }

    private void Paint(Vector2 tiledPos, Vector2 pos, HallContent content)
    {
        var newTile = Instantiate(_tilePrefab.gameObject, Vector2.zero, Quaternion.identity,
            _tilesParent.GetComponent<RectTransform>());
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
        HallSelected = new Hall();
        for (int i = 0; i < _tilesParent.transform.childCount; i++)
            Destroy(_tilesParent.transform.GetChild(i).gameObject);
        _textGORefreshing.SetActive(true);
        _hallPreview.SetActive(false);
        _modeSwitchEdit.gameObject.SetActive(false);
        _modeSwitchNew.gameObject.SetActive(true);
        StartCoroutine(InitializeAllHalls());
    }

    private IEnumerator InitializeAllHalls()
    {
        _cachedHalls = new List<Hall>();

        yield return GetAllHalls();
        ParseAllHallsIntoCache();
        CreateAllHallListings();

        _textGORefreshing.SetActive(false);
        _hallPreview.SetActive(true);
    }

    private IEnumerator GetAllHalls()
    {
        _responseText = "";
        string phpFileName = "get_all_halls.php";
        yield return _queriesToPhp.GetRequest(phpFileName, _responseCallback);
    }

    private void ParseAllHallsIntoCache()
    {
        if (string.IsNullOrEmpty(_responseText) || _responseText.Split(" ")[0] == "<br")
            return;
        Debug.Log(_responseText);
        var hallsData = _responseText.Split(";");
        foreach (var hall in hallsData)
        {
            if (string.IsNullOrEmpty(hall))
                continue;
            Hall newHall = new Hall();
            var hallData = hall.Split("|");
            newHall.hnum = Int32.Parse(hallData[0]);
            newHall.name = hallData[1];
            newHall.sizex = Int32.Parse(hallData[2]);
            newHall.sizez = Int32.Parse(hallData[3]);
            newHall.is_date_b = Int32.Parse(hallData[4]) == 1;
            newHall.is_date_e = Int32.Parse(hallData[5]) == 1;
            newHall.date_begin = hallData[6];
            newHall.date_end = hallData[7];
            newHall.is_maintained = Int32.Parse(hallData[8]) == 1;
            newHall.is_hidden = Int32.Parse(hallData[9]) == 1;
            newHall.time_added = hallData[10];
            Debug.Log("Added new hall: " + newHall.name);
            _cachedHalls.Add(newHall);
        }
    }

    private void CreateAllHallListings()
    {
        foreach (var hall in _cachedHalls)
        {
            var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                _hallListingsParent);
            newInstance.gameObject.name = hall.hnum + " - " + hall.name;
            newInstance.GetComponentInChildren<TextMeshProUGUI>().text = hall.name;
            newInstance.onClick.AddListener(() => SelectHallFromButton(hall.hnum, newInstance.gameObject));
        }
        
    }

private void SelectHallFromButton(int onum, GameObject linkGO)
    {
        for (int i = 0; i < _hallListingsParent.transform.childCount; i++)
        {
            if (_hallListingsParent.GetChild(i).gameObject == linkGO)
            {
                _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.green;
                _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.black; 
            }
            else
            {
                _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.gray;
                _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.white; 
            }
        }
        SelectHall(onum);
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
