using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminEditMode : MonoBehaviour
{
    [SerializeField] private AdminViewMode _adminView;
    [SerializeField] private RectTransform _cursorTile;
    [SerializeField] private RectTransform _imagePreview;

    void Update()
    {
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
            Mathf.FloorToInt((0.4f) * (windowSize.x / tileSize)) * tileSize + addPosX,
            Mathf.FloorToInt((0.5f) * (windowSize.y / tileSize)) * tileSize + addPosY
        );
        Vector2 tiledMousePos = new Vector2
        (
            Mathf.FloorToInt((absoluteMousePos.x / windowSize.x) * (windowSize.x / tileSize)) * tileSize + tileSize/2,
            Mathf.FloorToInt((absoluteMousePos.y / windowSize.y) * (windowSize.y / tileSize)) * tileSize + tileSize/4
        );
        if (absoluteMousePos.x < 0.75f * windowSize.x)
            _cursorTile.anchoredPosition = tiledMousePos;
        else
            _cursorTile.anchoredPosition = -windowSize;
    }
}
