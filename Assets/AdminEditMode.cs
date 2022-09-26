using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminEditMode : MonoBehaviour
{
    [SerializeField] private Color32 _doorColor, _frameColor, _infoColor, _rubberColor;
    [SerializeField] private AdminViewMode _adminView;
    [SerializeField] private RectTransform _paintsParent;
    [SerializeField] private RectTransform _cursorTile;
    [SerializeField] private RectTransform _imagePreview;
    private int _currentTool = -999;

    private void Start()
    {
        SelectTool(-1);
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
            if (Input.GetMouseButtonDown(0))
            {
                Paint();
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < _paintsParent.childCount; i++)
            Destroy(_paintsParent.GetChild(i).gameObject);
    }

    private void Paint()
    {
        var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity, _paintsParent);
        newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchoredPosition = _cursorTile.anchoredPosition;
        newTile.GetComponent<Image>().color = _cursorTile.GetComponent<Image>().color;
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
