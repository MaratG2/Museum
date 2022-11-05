using System;
using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
using Npgsql;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminEditMode : MonoBehaviour
{
    [SerializeField] private Color32 _doorColor, _frameColor, _infoColor, _rubberColor;
    [SerializeField] private AdminViewMode _adminView;
    [SerializeField] private RectTransform _paintsParent;
    [SerializeField] private RectTransform _cursorTile;
    [SerializeField] private RectTransform _imagePreview;
    [SerializeField] private Toggle _toggleMaintained, _toggleHidden;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private CanvasGroup _changePropertiesGroup;
    [SerializeField] private TMP_InputField _propertiesName;
    [SerializeField] private TMP_InputField _propertiesUrl;
    [SerializeField] private TMP_InputField _propertiesDesc;
    
    private int _currentTool = -999;
    private int[][] _hallPlan;
    private Vector2 _startTilePos = Vector2.zero;
    private List<Vector2> posToDelete = new List<Vector2>();
    private Tile _tileSelected;
    
    [System.Serializable]
    public struct HallContent
    {
        public int onum;
        public int cnum;
        public string title;
        public string image_url;
        public int pos_x;
        public int pos_z;
        public string combined_pos;
        public int type;
        public string image_desc;
    }
    
    private void Start()
    {
        _nameText.text = "";
        SelectTool(-1);
    }

    public void Refresh()
    {
        if (_adminView.HallSelected.sizex == 0)
        {
            _nameText.text = "";
            _toggleMaintained.interactable = false;
            _toggleHidden.interactable = false;
            return;
        }
        _hallPlan = new int[_adminView.HallSelected.sizex][];
        for (int i = 0; i < _adminView.HallSelected.sizex; i++)
        {
            _hallPlan[i] = new int[_adminView.HallSelected.sizez];
            for (int j = 0; j < _adminView.HallSelected.sizez; j++)
                _hallPlan[i][j] = -1;
        }

        _toggleMaintained.interactable = true;
        _toggleHidden.interactable = true;
        posToDelete = new List<Vector2>();
        _nameText.text = _adminView.HallSelected.name;
        _toggleMaintained.isOn = Convert.ToBoolean(_adminView.HallSelected.is_maintained);
        _toggleHidden.isOn = Convert.ToBoolean(_adminView.HallSelected.is_hidden);
        _startTilePos = Vector2.zero;
    }

    private void FindLeftBottomTile()
    {
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
     
        float addPosX = 0, addPosY = tileSize / 4;
        if(_adminView.HallSelected.sizez % 2 == 0)
            addPosY += -tileSize / 4;
        if (_adminView.HallSelected.sizex % 2 != 0)
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
                    return;
                }
            }
        }
    }

    public void DeleteHall()
    {
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string sql = "DELETE FROM options "
            + "WHERE onum = " + _adminView.HallSelected.onum;
        dbcmd.Prepare();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteNonQuery();
        ClearAll();
        _nameText.text = "";
        _adminView.HallSelected = new AdminNewMode.HallOptions();
        SelectTool(-1);
    }

    public void SaveHall()
    {
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string sql = "UPDATE options"
                     + " SET is_maintained = " + _toggleMaintained.isOn
                     + ", is_hidden = " + _toggleHidden.isOn
                     + " WHERE onum = " + _adminView.HallSelected.onum;
        dbcmd.Prepare();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteNonQuery();

        for (int i = 0; i < _paintsParent.childCount; i++)
        {
            var c = _paintsParent.GetChild(i).GetComponent<Tile>().hallContent;
            c.onum = _adminView.HallSelected.onum;
            Debug.Log(c.image_url);
            string sqlInsert = "INSERT INTO contents (onum, title, image_url, pos_x, pos_z, combined_pos, image_desc, type)" +
                               " VALUES(" + c.onum + ",'" + c.title + "','" + c.image_url + "'," + c.pos_x + ',' + c.pos_z + ",'" +
                               c.combined_pos + "','" + c.image_desc + "'," + c.type + ")" +
                               " ON CONFLICT (combined_pos) DO UPDATE" +
                               " SET title = EXCLUDED.title, image_url = EXCLUDED.image_url, pos_x = EXCLUDED.pos_x, pos_z = EXCLUDED.pos_z, " +
                               "combined_pos = EXCLUDED.combined_pos, image_desc = EXCLUDED.image_desc, type = EXCLUDED.type";
            dbcmd.Prepare();
            dbcmd.CommandText = sqlInsert;
            dbcmd.ExecuteNonQuery();
        }

        foreach (var posDel in posToDelete)
        {
            string sqlDelete = "DELETE FROM contents"
                               + " WHERE combined_pos = '" + posDel.x + "_" + posDel.y + "'";
            dbcmd.Prepare();
            dbcmd.CommandText = sqlDelete;
            dbcmd.ExecuteNonQuery();
        }
    }

    private void SQLGetOptionsContents(int on)
    {
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string sql =
            "SELECT * FROM public.contents c" +
            " WHERE c.onum = " + on;
        dbcmd.CommandText = sql;
        NpgsqlDataReader reader = dbcmd.ExecuteReader();
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
        while (reader.Read())
        {
            int onum = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
            int cnum = (reader.IsDBNull(1)) ? 0 : reader.GetInt32(1);
            string title = (reader.IsDBNull(2)) ? "NULL" : reader.GetString(2);
            string image_url = (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3);
            int pos_x = (reader.IsDBNull(4)) ? 0 : reader.GetInt32(4);
            int pos_z = (reader.IsDBNull(5)) ? 0 : reader.GetInt32(5);
            string combined_pos = (reader.IsDBNull(6)) ? "NULL" : reader.GetString(6);
            int type = (reader.IsDBNull(7)) ? 0 : reader.GetInt32(7);
            string image_desc = (reader.IsDBNull(8)) ? "NULL" : reader.GetString(8);
          
            HallContent newContent = new HallContent();
            newContent.onum = onum;
            newContent.cnum = cnum;
            newContent.title = title;
            newContent.image_url = image_url;
            newContent.pos_x = pos_x;
            newContent.pos_z = pos_z;
            newContent.combined_pos = combined_pos;
            newContent.type = type;
            newContent.image_desc = image_desc;

            SelectTool(newContent.type);
            Vector2 tilePos = _startTilePos + new Vector2(newContent.pos_x, newContent.pos_z);
            Vector2 drawPos = new Vector2
            (
                tilePos.x * tileSize,
                tilePos.y * tileSize
            );
            Paint(tilePos, drawPos, true, newContent);
        }
        
        reader.Close();
        reader = null;
    }

    void Update()
    {
        if (_adminView.HallSelected.sizex == 0 || _adminView.HallSelected.sizez == 0)
            return;
        
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        Vector2 absoluteMousePos = Input.mousePosition;
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
        _cursorTile.sizeDelta = new Vector2(tileSize, tileSize);

        float addPosX = 0, addPosY = tileSize / 4;
        if(_adminView.HallSelected.sizez % 2 == 0)
            addPosY = -tileSize / 4;
        if (_adminView.HallSelected.sizex % 2 != 0)
            addPosX = tileSize / 2;

        _imagePreview.anchoredPosition = new Vector2
        (
            Mathf.FloorToInt((0.35f) * (windowSize.x / tileSize)) * tileSize + addPosX,
            Mathf.FloorToInt((0.55f) * (windowSize.y / tileSize)) * tileSize + addPosY
        );
        Vector2 tiledMousePos = new Vector2
        (
            Mathf.FloorToInt((absoluteMousePos.x / windowSize.x) * (windowSize.x / tileSize)) * tileSize + tileSize/2,
            Mathf.FloorToInt(((absoluteMousePos.y + tileSize / 4) / windowSize.y) * (windowSize.y / tileSize)) * tileSize + tileSize/4
        );

        bool isOverPreview = false;
        GameObject[] casted = AdminHallPreview.RaycastUtilities.UIRaycasts(
            AdminHallPreview.RaycastUtilities.ScreenPosToPointerData(absoluteMousePos));
        foreach (var c in casted)
        {
            if (c.GetComponent<AdminHallPreview>())
                isOverPreview = true;
        }
        
        if (absoluteMousePos.x < 0.75f * windowSize.x && isOverPreview)
            _cursorTile.anchoredPosition = tiledMousePos;
        else
            _cursorTile.anchoredPosition = -windowSize;
        
        if(_startTilePos == Vector2.zero)
        {
            _startTilePos = Vector2.one;
            _hallPlan = new int[_adminView.HallSelected.sizex][];
            for (int i = 0; i < _adminView.HallSelected.sizex; i++)
            {
                _hallPlan[i] = new int[_adminView.HallSelected.sizez];
                for (int j = 0; j < _adminView.HallSelected.sizez; j++)
                    _hallPlan[i][j] = -1;
            }
            posToDelete = new List<Vector2>();
            FindLeftBottomTile();
            SQLGetOptionsContents(_adminView.HallSelected.onum);
        }
        
        if(_currentTool is 0 or 1 or 2 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Paint(tiledMousePos/tileSize, _cursorTile.anchoredPosition);
            }
        }
        if(_currentTool is 7 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tiledPos = tiledMousePos / tileSize;
                Vector2 tileRealPos = new Vector2(Mathf.FloorToInt(tiledPos.x - _startTilePos.x),
                    Mathf.FloorToInt(tiledPos.y - _startTilePos.y));
           
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if(tileChange && tileChange.hallContent.pos_x == tileRealPos.x && tileChange.hallContent.pos_z == tileRealPos.y)
                    {
                        Debug.Log("tileChange " + i);
                        if (tileChange.hallContent.type == 0)
                        {
                            Debug.Log("Tile Change Door" + i);
                        }
                        if (tileChange.hallContent.type == 1)
                        {
                            Debug.Log("Tile Change Painting" + i);
                            _changePropertiesGroup.alpha = 1;
                            _changePropertiesGroup.interactable = true;
                            _changePropertiesGroup.blocksRaycasts = true;
                            _propertiesName.text = tileChange.hallContent.title;
                            _propertiesUrl.text = tileChange.hallContent.image_url;
                            _propertiesDesc.text = tileChange.hallContent.image_desc;
                            _tileSelected = tileChange;
                        }
                        if (tileChange.hallContent.type == 2)
                        {
                            Debug.Log("Tile Change Info" + i);
                        }
                    }
                }
            }
        }
        if(_currentTool is 8 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tiledPos = tiledMousePos / tileSize;
                _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][
                    Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] = -1;
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if(tileDelete && tileDelete.hallContent.pos_x == Mathf.FloorToInt(tiledPos.x - _startTilePos.x) 
                       && tileDelete.hallContent.pos_z == Mathf.FloorToInt(tiledPos.y - _startTilePos.y))
                    {
                        Debug.Log("Delete " + i);
                        posToDelete.Add(new Vector2(tileDelete.hallContent.pos_x, tileDelete.hallContent.pos_z));
                        Destroy(tileDelete.gameObject);
                    }
                }
            }
        }
    }

    public void HidePropertiesGroup()
    {
        _changePropertiesGroup.alpha = 0;
        _changePropertiesGroup.interactable = false;
        _changePropertiesGroup.blocksRaycasts = false;
    }

    public void SaveProperties()
    {
        if (!_tileSelected)
            return;
        _tileSelected.hallContent.title = _propertiesName.text;
        _tileSelected.hallContent.image_url = _propertiesUrl.text;
        _tileSelected.hallContent.image_desc = _propertiesDesc.text;
        
        Debug.Log("SAVE: " + _propertiesUrl.text + " | " + _tileSelected.hallContent.image_url);
        HidePropertiesGroup();
    }
    
    public void ClearAll()
    {
        for (int i = 0; i < _paintsParent.childCount; i++)
            Destroy(_paintsParent.GetChild(i).gameObject);
        posToDelete = new List<Vector2>();
    }

    private void Paint(Vector2 tiledPos, Vector2 pos, bool hasStruct = false, HallContent content = new HallContent())
    {
        var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity, _paintsParent);
        newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchoredPosition = pos;
        newTile.GetComponent<Image>().color = _cursorTile.GetComponent<Image>().color;
        _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] =
            _currentTool;
        Tile tileInstance = newTile.GetComponent<Tile>();
        tileInstance.hallContent.image_desc = "desc";
        tileInstance.hallContent.image_url = "url";
        tileInstance.hallContent.title = "title";
        tileInstance.hallContent.type = _currentTool;
        tileInstance.hallContent.pos_x = Mathf.FloorToInt(tiledPos.x - _startTilePos.x);
        tileInstance.hallContent.pos_z = Mathf.FloorToInt(tiledPos.y - _startTilePos.y);
        tileInstance.hallContent.combined_pos = tileInstance.hallContent.pos_x + "_" + tileInstance.hallContent.pos_z;
        for (int i = 0; i < posToDelete.Count; i++)
        {
            if (tileInstance.hallContent.combined_pos == posToDelete[i].x + "_" + posToDelete[i].y)
                posToDelete.RemoveAt(i);
        }
        //tileInstance.hallContent.uid = uid;
        if (hasStruct)
            tileInstance.hallContent = content;
        tileInstance.Setup();
    }

    public void SelectTool(int tool)
    {
        _currentTool = tool;
        switch (tool)
        {
            case -1:
                _cursorTile.GetComponent<Image>().color = Color.clear;
                break;
            case 0:
                _cursorTile.GetComponent<Image>().color = _doorColor;
                break;
            case 1:
                _cursorTile.GetComponent<Image>().color = _frameColor;
                break;
            case 2:
                _cursorTile.GetComponent<Image>().color = _infoColor;
                break;
            case 7:
                _cursorTile.GetComponent<Image>().color = Color.clear;
                break;
            case 8:
                _cursorTile.GetComponent<Image>().color = _rubberColor;
                break;
        }
    }
}
