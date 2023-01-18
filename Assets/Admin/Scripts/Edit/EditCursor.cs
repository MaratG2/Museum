using Admin.Utility;
using Admin.View;
using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за просчет положения и отображение курсора в интерфейсе.
    /// </summary>
    public class EditCursor : MonoBehaviour
    {
        [SerializeField] private RectTransform _cursorTile;
        [SerializeField] private CanvasGroup _changePropertiesGroup;
        public RectTransform CursorTile => _cursorTile;
        private TilesDrawer _tilesDrawer;
        public Vector2 TiledMousePos { get; private set; }
        public Vector2 TileMousePos { get; private set; }
        public Vector2 TiledHallMousePos { get; private set; }
        public Vector2 TileHallMousePos { get; private set; }
        private float _tileSize;
        private Vector2 _windowSize;
        private Vector2 _absoluteMousePos;
        private Vector2 _startTilePos;
        private bool _isCursorLock;

        private void Awake()
        {
            _tilesDrawer = GetComponent<TilesDrawer>();
        }
        
        private void OnEnable()
        {
            _tilesDrawer.OnStartTileFound += startTile => _startTilePos = startTile;
        }

        private void OnDisable()
        {
            _tilesDrawer.OnStartTileFound -= startTile => TiledHallMousePos = startTile;
        }

        public void ChangeCursorLock(bool setTo)
        {
            _isCursorLock = setTo;
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
            TiledMousePos = new Vector2
            (
                Mathf.FloorToInt((_absoluteMousePos.x / _windowSize.x) * (_windowSize.x / _tileSize)) * _tileSize +
                _tileSize / 2,
                Mathf.FloorToInt(((_absoluteMousePos.y + _tileSize / 4) / _windowSize.y) * (_windowSize.y / _tileSize)) *
                _tileSize + _tileSize / 4
            );
            TileMousePos = TiledMousePos / _tileSize;
            TiledHallMousePos = TiledMousePos - _startTilePos;
            TileHallMousePos = TileMousePos - _startTilePos;
            
            bool isOverPreview = CheckIfIsOverPreview();
            
            if (isOverPreview && _absoluteMousePos.x < 0.75f * _windowSize.x)
                _cursorTile.anchoredPosition = TiledMousePos;
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