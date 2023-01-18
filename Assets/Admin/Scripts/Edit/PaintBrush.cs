using Admin.GenerationMap;
using Admin.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечающий за рисование выбранной иконки из палитры на 2D плане зала, а также установление начального состояния нарисованного предмета музея.
    /// </summary>
    public class PaintBrush : MonoBehaviour
    {
        [SerializeField] private Transform _paintsParent;
        [SerializeField] private Button _doorTool;
        private ToolSelector _toolSelector;
        private EditCursor _editCursor;
        private HallEditor _hallEditor;
        private EditDecoration _editDecoration;
        
        private void Awake()
        {
            _toolSelector = GetComponent<ToolSelector>();
            _editCursor = GetComponent<EditCursor>();
            _hallEditor = GetComponent<HallEditor>();
            _editDecoration = GetComponent<EditDecoration>();
        }

        private void Update()
        {
            BlockDoorToolIfNeeded();
            if (_toolSelector.CanDraw() && Input.GetMouseButtonDown(0))
                Paint(_editCursor.TileHallMousePos, _editCursor.TiledMousePos);
        }

        private void BlockDoorToolIfNeeded()
        {
            bool turnToTrue = true;
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                if (tileChange.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
                    turnToTrue = false;
            }

            _doorTool.interactable = turnToTrue;
            _toolSelector.IsDoorBlock = !turnToTrue;
        }
        
        private void Paint(Vector2 hallPos, Vector2 globalPos, bool hasStruct = false, HallContent content = new())
        {
            if (!_hallEditor.IsPlanAtPosEmpty(hallPos))
                return;
            
            var newTileGO = Instantiate(_editCursor.CursorTile.gameObject, globalPos, Quaternion.identity,
                _paintsParent);
            newTileGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            newTileGO.GetComponent<RectTransform>().anchorMax = Vector2.zero;
            newTileGO.GetComponent<RectTransform>().anchoredPosition = globalPos;
            newTileGO.GetComponent<Image>().color = _editCursor.CursorTile.GetComponent<Image>().color;
            
            _hallEditor.SetHallPlan(hallPos, _toolSelector.CurrentTool);
            
            Tile newTile = newTileGO.GetComponent<Tile>();
            newTile.hallContent.type = _toolSelector.CurrentTool;
            newTile.hallContent.pos_x = (int)hallPos.x;
            newTile.hallContent.pos_z = (int)hallPos.y;
            newTile.hallContent.combined_pos = $"{(int)hallPos.x}_{(int)hallPos.y}";

            ProcessMedia(newTile);
            ProcessDecoration(newTile);
            ProcessSpawnPoint(newTile);
            _hallEditor.RemoveFromPosToDelete(newTile.hallContent.combined_pos);

            if (hasStruct)
                newTile.hallContent = content;

            newTile.Setup();
        }

        private void ProcessMedia(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.Picture.Id
                && tile.hallContent.type != ExhibitsConstants.Video.Id)
                return;
            
            tile.hallContent.image_desc = "desc";
            tile.hallContent.image_url = "url";
            tile.hallContent.title = "title";
        }

        private void ProcessDecoration(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.Decoration.Id)
                return;

            var decorNum = _editDecoration.DecorationsDropdown.value;
            tile.hallContent.title = decorNum.ToString();
            tile.hallContent.image_url = "Decoration";
            tile.hallContent.image_desc = _editDecoration.DecorationsDropdown.options[decorNum].text;
        }

        private void ProcessSpawnPoint(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.SpawnPoint.Id)
                return;

            _toolSelector.SelectTool(-3);
            _toolSelector.IsDoorBlock = true;
        }
    }
}