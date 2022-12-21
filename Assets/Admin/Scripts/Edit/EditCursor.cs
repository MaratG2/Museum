using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Admin.Edit
{
    public class EditCursor : MonoBehaviour
    {
        [SerializeField] private RectTransform _cursorTile;
        private Tile _tileSelected;
        public RectTransform CursorTile => _cursorTile;
        public Tile TileSelected => _tileSelected;

        private void Update()
        {
            UpdateCursorPosition();
        }
        
        public bool IsCursorReady()
        {
            return _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0;
        }

        private void UpdateCursorPosition()
        {
            Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            Vector2 absoluteMousePos = Input.mousePosition;
            _tileSize = _imagePreview.sizeDelta.x / _hallViewer.HallSelected.sizex;
            _cursorTile.sizeDelta = new Vector2(_tileSize, _tileSize);

            float addPosX = 0, addPosY = _tileSize / 4;
            if (_hallViewer.HallSelected.sizez % 2 == 0)
                addPosY = -_tileSize / 4;
            if (_hallViewer.HallSelected.sizex % 2 != 0)
                addPosX = _tileSize / 2;

            _imagePreview.anchoredPosition = new Vector2
            (
                Mathf.FloorToInt((0.35f) * (windowSize.x / _tileSize)) * _tileSize + addPosX,
                Mathf.FloorToInt((0.55f) * (windowSize.y / _tileSize)) * _tileSize + addPosY
            );
            _tiledMousePos = new Vector2
            (
                Mathf.FloorToInt((absoluteMousePos.x / windowSize.x) * (windowSize.x / _tileSize)) * _tileSize +
                _tileSize / 2,
                Mathf.FloorToInt(((absoluteMousePos.y + _tileSize / 4) / windowSize.y) * (windowSize.y / _tileSize)) *
                _tileSize + _tileSize / 4
            );
            
            bool isOverPreview = false;
            GameObject[] casted =
                RaycastUtilities.UIRaycasts(RaycastUtilities.ScreenPosToPointerData(absoluteMousePos));
            foreach (var c in casted)
            {
                if (c.GetComponent<HallPreviewResizer>())
                    isOverPreview = true;
            }

            if (!_isCursorLock)
            {
                if (absoluteMousePos.x < 0.75f * windowSize.x && isOverPreview)
                    _cursorTile.anchoredPosition = _tiledMousePos;
                else
                    _cursorTile.anchoredPosition = -windowSize;
            }
            //Debug.Log(_tiledMousePos/_tileSize);
        }
    }
}