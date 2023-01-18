using Admin.GenerationMap;
using MaratG2.Extensions;
using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за редактирование (изменение) предмета на месте курсора, его обработку в зависимости от его типа.
    /// </summary>
    public class EditBrush : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _changePropertiesGroup;
        [SerializeField] private Transform _paintsParent;
        public Tile TileSelected { get; private set; }
        private EditCursor _editCursor;
        private EditMedia _editMedia;
        private EditInfoBox _editInfoBox;
        private EditDecoration _editDecoration;

        private void Awake()
        {
            _editCursor = GetComponent<EditCursor>();
            _editMedia = GetComponent<EditMedia>();
            _editInfoBox = GetComponent<EditInfoBox>();
            _editDecoration = GetComponent<EditDecoration>();
        }

        public void Edit()
        {
            Vector2 tileRealPos = _editCursor.TileHallMousePos;
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                if (!tileChange || tileChange.hallContent.combined_pos != $"{tileRealPos.x}_{tileRealPos.y}")
                    continue;
                ProcessPicture(tileChange);
                ProcessInfoBox(tileChange);
                ProcessVideo(tileChange);
                ProcessDecoration(tileChange);
            }
        }

        private void Processed(Tile tile)
        {
            _editCursor.ChangeCursorLock(true);
            _changePropertiesGroup.SetActive(true);
            TileSelected = tile;
        }
        private void ProcessPicture(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.Picture.Id)
                return;
            
            Processed(tile);
            _editMedia.ShowMedia(tile.hallContent, true);
        }
        
        private void ProcessInfoBox(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.InfoBox.Id)
                return;
            
            Processed(tile);
            _editInfoBox.ShowMedia(tile.hallContent);
        }
        
        private void ProcessVideo(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.Video.Id)
                return;
            
            Processed(tile);
            _editMedia.ShowMedia(tile.hallContent, false);
        }
        
        private void ProcessDecoration(Tile tile)
        {
            if (tile.hallContent.type != ExhibitsConstants.Decoration.Id)
                return;
            
            Processed(tile);
            _editDecoration.ShowMedia(tile.hallContent);
        }
    }
}