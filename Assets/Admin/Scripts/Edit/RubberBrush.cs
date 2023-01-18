using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечающий за удаление иконки на месте курсора при выбранном инструменте резинки.
    /// </summary>
    public class RubberBrush : MonoBehaviour
    {
        [SerializeField] private Transform _paintsParent;
        private HallEditor _hallEditor;
        private EditCursor _editCursor;

        private void Awake()
        {
            _hallEditor = GetComponent<HallEditor>();
            _editCursor = GetComponent<EditCursor>();
        }

        public void Delete()
        {
            Vector2 planPos = _editCursor.TileHallMousePos;
            _hallEditor.SetHallPlan(planPos, -1);
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                if (tileDelete && tileDelete.hallContent.combined_pos == $"{planPos.x}_{planPos.y}")
                {
                    Debug.Log("Delete: " + i);
                    _hallEditor.AddToPosToDelete(new Vector2(tileDelete.hallContent.pos_x,
                        tileDelete.hallContent.pos_z));
                    Destroy(tileDelete.gameObject);
                }
            }
        }
    }
}