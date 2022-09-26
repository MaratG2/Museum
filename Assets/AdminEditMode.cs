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
            _hallPlan[i] = new int[_adminView.HallSelected.sizez];
        
        _toggleMaintained.interactable = true;
        _toggleHidden.interactable = true;

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
                , "uid", _paintsParent.GetChild(i).GetComponent<Tile>().hallContent.uid.ToString()
                , jsonPlayer, true, true);
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
                _hallPlan[i] = new int[_adminView.HallSelected.sizez];
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
    }

    public void ClearAll()
    {
        for (int i = 0; i < _paintsParent.childCount; i++)
            Destroy(_paintsParent.GetChild(i).gameObject);
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
        newTile.GetComponent<Tile>().hallContent.image_desc = "desc";
        newTile.GetComponent<Tile>().hallContent.image_url = "url";
        newTile.GetComponent<Tile>().hallContent.title = "title";
        newTile.GetComponent<Tile>().hallContent.type = _currentTool.ToString();
        newTile.GetComponent<Tile>().hallContent.pos_x = Mathf.FloorToInt(tiledPos.x - _startTilePos.x);
        newTile.GetComponent<Tile>().hallContent.pos_z = Mathf.FloorToInt(tiledPos.y - _startTilePos.y);
        newTile.GetComponent<Tile>().hallContent.uid = uid;
        if (hasStruct)
            newTile.GetComponent<Tile>().hallContent = content;
        newTile.GetComponent<Tile>().Setup();
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
            case 8:
                _cursorTile.GetComponent<Image>().color = _rubberColor;
                break;
        }
    }
}
