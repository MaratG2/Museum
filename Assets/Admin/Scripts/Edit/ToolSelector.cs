using Admin.GenerationMap;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за выбор инструмента из палитры, а также изменение иконки курсора.
    /// </summary>
    public class ToolSelector : MonoBehaviour
    {
        [SerializeField] private Sprite _doorSprite,
            _frameSprite,
            _infoSprite,
            _cupSprite,
            _medalSprite,
            _rubberSprite,
            _videoSprite,
            _decorSprite,
            _selectSprite;
        public int CurrentTool { get; private set; } = -999;
        private EditCursor _cursor;
        public bool IsDoorBlock { get; set; }
        
        private void Awake()
        {
            _cursor = GetComponent<EditCursor>();
        }

        private void Start()
        {
            SelectTool(-1);    
        }

        public bool CanDraw()
        {
            return (CurrentTool == ExhibitsConstants.SpawnPoint.Id
                    || CurrentTool == ExhibitsConstants.Picture.Id
                    || CurrentTool == ExhibitsConstants.InfoBox.Id
                    || CurrentTool == ExhibitsConstants.Cup.Id
                    || CurrentTool == ExhibitsConstants.Medal.Id
                    || CurrentTool == ExhibitsConstants.Video.Id
                    || CurrentTool == ExhibitsConstants.Decoration.Id)
                   && _cursor.IsCursorReady();
        }

        public void SelectTool(int tool)
        {
            CurrentTool = tool;
            var _cursorImage = _cursor.CursorTile.GetComponent<Image>();
            _cursorImage.color = Color.white;
            switch (CurrentTool)
            {
                case -3:
                    _cursorImage.sprite = _selectSprite;
                    _cursorImage.raycastTarget = false;
                    break;
                case -2:
                    _cursorImage.sprite = _rubberSprite;
                    break;
                case -1:
                    _cursorImage.sprite = null;
                    _cursorImage.color = Color.clear;
                    break;
            }

            if (CurrentTool == ExhibitsConstants.Picture.Id)
                _cursorImage.sprite = _frameSprite;
            else if (!IsDoorBlock && CurrentTool == ExhibitsConstants.SpawnPoint.Id)
                _cursorImage.sprite = _doorSprite;
            else if (CurrentTool == ExhibitsConstants.InfoBox.Id)
                _cursorImage.sprite = _infoSprite;
            else if (CurrentTool == ExhibitsConstants.Cup.Id)
                _cursorImage.GetComponent<Image>().sprite = _cupSprite;
            else if (CurrentTool == ExhibitsConstants.Medal.Id)
                _cursorImage.GetComponent<Image>().sprite = _medalSprite;
            else if (CurrentTool == ExhibitsConstants.Video.Id)
                _cursorImage.GetComponent<Image>().sprite = _videoSprite;
            else if (CurrentTool == ExhibitsConstants.Decoration.Id)
                _cursorImage.GetComponent<Image>().sprite = _decorSprite;
        }
    }
}