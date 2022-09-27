using System;
using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
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
    
    private int _currentTool = -999;
    private int[][] _hallPlan;
    private Vector2 _startTilePos = Vector2.zero;
    private bool _toDelete;
    private bool _toUpdate;
    private int uid = 0;
    private List<Vector2> posToDelete = new List<Vector2>();
    
    [System.Serializable]
    public struct HallContent
    {
        public int uid;
        public string type;
        public string title;
        public string image_url;
        public string image_desc;
        public int pos_x;
        public int pos_z;
        public string combined_pos;
    }
    
    private void Start()
    {
        _nameText.text = "";
        SelectTool(-1);
    }
    
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
        _toDelete = true;
        Drive.GetObjectsByField("Options", "name", _adminView.HallSelected.name, true);
        ClearAll();
        _nameText.text = "";
        _adminView.HallSelected = new AdminNewMode.HallOptions();
        SelectTool(-1);
    }

    public void SaveHall()
    {
        _toUpdate = true;
        Drive.GetObjectsByField("Options", "name", _adminView.HallSelected.name, true);
        for (int i = 0; i < _paintsParent.childCount; i++)
        {
            string jsonPlayer = JsonUtility.ToJson(_paintsParent.GetChild(i).GetComponent<Tile>().hallContent);
            Drive.UpdateObjects(_adminView.HallSelected.name
                , "combined_pos", _paintsParent.GetChild(i).GetComponent<Tile>().hallContent.combined_pos
                , jsonPlayer, true, true);
            
        }

        foreach (var posDel in posToDelete)
        {
            Drive.DeleteObjects(_adminView.HallSelected.name, "combined_pos", posDel.x + "_" + posDel.y,true);
        }
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        Debug.Log(dataContainer.msg);

        
        if (dataContainer.QueryType == Drive.QueryType.getTable)
        {
            string rawJSon2 = dataContainer.payload;
            Debug.Log("GET");
            if (string.Compare(dataContainer.objType, _adminView.HallSelected.name) == 0)
            {
                HallContent[] players = JsonHelper.ArrayFromJson<HallContent>(rawJSon2);
                float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
                for (int i = 0; i < players.Length; i++)
                {
                    SelectTool(Int32.Parse(players[i].type));
                    Vector2 tilePos = _startTilePos + new Vector2(players[i].pos_x, players[i].pos_z);
                    Vector2 drawPos = new Vector2
                    (
                        tilePos.x * tileSize,
                        tilePos.y * tileSize
                    );
                    Paint(tilePos, drawPos, true, players[i]);
                    Debug.Log("IN");
                }
            }
        }
        
        if (dataContainer.QueryType == Drive.QueryType.getObjectsByField)
        {
            string rawJSon = dataContainer.payload;
            Debug.Log(rawJSon);

            // Check if the type is correct.
            if (string.Compare(dataContainer.objType, "Options") == 0)
            {
                // Parse from json to the desired object type.
                AdminNewMode.HallOptions[] players = JsonHelper.ArrayFromJson<AdminNewMode.HallOptions>(rawJSon);

                for (int i = 0; i < players.Length; i++)
                {
                    if(_toDelete)
                    {
                        players[i].is_deleted = true.ToString();
                        string jsonPlayer = JsonUtility.ToJson(players[i]);
                        Drive.UpdateObjects("Options", "name", players[i].name, jsonPlayer, false, true);
                        Debug.Log("Deleted");
                    }

                    if (_toUpdate)
                    {
                        players[i].is_maintained = _toggleMaintained.isOn.ToString();
                        players[i].is_hidden = _toggleHidden.isOn.ToString();
                        string jsonPlayer = JsonUtility.ToJson(players[i]);
                        Drive.UpdateObjects("Options", "name", players[i].name, jsonPlayer, false, true);
                    }
                }
            }
            _toDelete = false;
            _toUpdate = false;
        }
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
            Debug.Log("GetStart - " + _adminView.HallSelected.name);
            Drive.GetTable(_adminView.HallSelected.name, true);
        }
        
        if(_currentTool is 0 or 1 or 2 && _cursorTile.anchoredPosition.x > 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Paint(tiledMousePos/tileSize, _cursorTile.anchoredPosition);
            }
        }
        if(_currentTool is 7 && _cursorTile.anchoredPosition.x > 1)
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
                        if (tileChange.hallContent.type == 0.ToString())
                        {
                            Debug.Log("Tile Change Door" + i);
                        }
                        if (tileChange.hallContent.type == 1.ToString())
                        {
                            Debug.Log("Tile Change Painting" + i);
                        }
                        if (tileChange.hallContent.type == 2.ToString())
                        {
                            Debug.Log("Tile Change Info" + i);
                        }
                    }
                }
            }
        }
        if(_currentTool is 8 && _cursorTile.anchoredPosition.x > 1)
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

    public void ClearAll()
    {
        for (int i = 0; i < _paintsParent.childCount; i++)
            Destroy(_paintsParent.GetChild(i).gameObject);
        posToDelete = new List<Vector2>();
        uid = 0;
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
        tileInstance.hallContent.type = _currentTool.ToString();
        tileInstance.hallContent.pos_x = Mathf.FloorToInt(tiledPos.x - _startTilePos.x);
        tileInstance.hallContent.pos_z = Mathf.FloorToInt(tiledPos.y - _startTilePos.y);
        tileInstance.hallContent.combined_pos = tileInstance.hallContent.pos_x + "_" + tileInstance.hallContent.pos_z;
        for (int i = 0; i < posToDelete.Count; i++)
        {
            if (tileInstance.hallContent.combined_pos == posToDelete[i].x + "_" + posToDelete[i].y)
                posToDelete.RemoveAt(i);
        }
        tileInstance.hallContent.uid = uid;
        if (hasStruct)
            tileInstance.hallContent = content;
        tileInstance.Setup();
        uid++;
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
            case 3:
                _cursorTile.GetComponent<Image>().color = Color.clear;
                break;
            case 8:
                _cursorTile.GetComponent<Image>().color = _rubberColor;
                break;
        }
    }
}
