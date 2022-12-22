using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using Admin.View;
using UnityEngine;

namespace Admin.Edit
{
    public class EditCursor : MonoBehaviour
    {
        [SerializeField] private RectTransform _cursorTile;
        [SerializeField] private CanvasGroup _changePropertiesGroup;
        public RectTransform CursorTile => _cursorTile;
        private TilesDrawer _tilesDrawer;
        private Vector2 _tiledMousePos;
        private float _tileSize;
        private Vector2 _windowSize;
        private Vector2 _absoluteMousePos;
        private bool _isCursorLock;

        private void Awake()
        {
            _tilesDrawer = GetComponent<TilesDrawer>();
        }

        private void Update()
        {
            if (_isCursorLock)
                return;
            
            ChangeCursorTileSize();
            UpdateCursorPosition();
        }
        
        public bool IsCursorReady()
        {
            return _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0;
        }

        private void ChangeCursorTileSize()
        {
            _windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            _absoluteMousePos = Input.mousePosition;
            _tileSize = _tilesDrawer.TileSize;
            _cursorTile.sizeDelta = new Vector2(_tileSize, _tileSize);
        }

        private void UpdateCursorPosition()
        {
            _tiledMousePos = new Vector2
            (
                Mathf.FloorToInt((_absoluteMousePos.x / _windowSize.x) * (_windowSize.x / _tileSize)) * _tileSize +
                _tileSize / 2,
                Mathf.FloorToInt(((_absoluteMousePos.y + _tileSize / 4) / _windowSize.y) * (_windowSize.y / _tileSize)) *
                _tileSize + _tileSize / 4
            );
            
            bool isOverPreview = CheckIfIsOverPreview();
            
            if (isOverPreview && _absoluteMousePos.x < 0.75f * _windowSize.x)
                _cursorTile.anchoredPosition = _tiledMousePos;
            else
                _cursorTile.anchoredPosition = -_windowSize;
        }

        private bool CheckIfIsOverPreview()
        {
            GameObject[] casted =
                RaycastUtilities.UIRaycasts(RaycastUtilities.ScreenPosToPointerData(_absoluteMousePos));
            foreach (var c in casted)
            {
                if (c.GetComponent<HallPreviewResizer>())
                    return true;
            }
            return false;
        }
    }
}