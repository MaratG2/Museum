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
    [SerializeField] private TMP_InputField _inputFieldName;
    
    private int _currentTool = -999;
    private int[][] _hallPlan;
    private Vector2 _startTilePos = Vector2.zero;

    private void Start()
    {
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
            _toggleMaintained.interactable = false;
            _toggleHidden.interactable = false;
            _inputFieldName.interactable = false;
            return;
        }
        _hallPlan = new int[_adminView.HallSelected.sizex][];
        for (int i = 0; i < _adminView.HallSelected.sizex; i++)
            _hallPlan[i] = new int[_adminView.HallSelected.sizez];
        
        _toggleMaintained.interactable = true;
        _toggleHidden.interactable = true;
        _inputFieldName.interactable = true;
        
        _toggleMaintained.isOn = Convert.ToBoolean(_adminView.HallSelected.is_maintained);
        _toggleHidden.isOn = Convert.ToBoolean(_adminView.HallSelected.is_hidden);
        _inputFieldName.text = _adminView.HallSelected.name;
        _startTilePos = Vector2.zero;
    }

    private void FindLeftBottomTile()
    {
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
        _cursorTile.sizeDelta = new Vector2(tileSize, tileSize);

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
        Drive.GetObjectsByField("Options", "name", _adminView.HallSelected.name, true);
        ClearAll();
        _adminView.HallSelected = new AdminNewMode.HallOptions();
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        Debug.Log(dataContainer.msg);

        // First check the type of answer.
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
                    players[i].is_deleted = true.ToString();
                    string jsonPlayer = JsonUtility.ToJson(players[i]);
                    Drive.UpdateObjects("Options", "name", players[i].name, jsonPlayer, false, true);
                    Debug.Log("Changed");
                }
            }
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

        if(_currentTool is 0 or 1 or 2 && _cursorTile.anchoredPosition.x > 1)
        {
            if(_startTilePos == Vector2.zero)
            {
                _hallPlan = new int[_adminView.HallSelected.sizex][];
                for (int i = 0; i < _adminView.HallSelected.sizex; i++)
                    _hallPlan[i] = new int[_adminView.HallSelected.sizez];
                FindLeftBottomTile();
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                Paint(tiledMousePos/tileSize);
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < _paintsParent.childCount; i++)
            Destroy(_paintsParent.GetChild(i).gameObject);
    }

    private void Paint(Vector2 tiledPos)
    {
        var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity, _paintsParent);
        newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchoredPosition = _cursorTile.anchoredPosition;
        newTile.GetComponent<Image>().color = _cursorTile.GetComponent<Image>().color;
        _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] =
            _currentTool;
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
