using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Admin.Edit
{
    public class RubberBrush : MonoBehaviour
    {
        [SerializeField] private Transform _paintsParent;
        private AdminEditMode _adminEditMode;
        private EditCursor _editCursor;

        private void Awake()
        {
            _adminEditMode = GetComponent<AdminEditMode>();
            _editCursor = GetComponent<EditCursor>();
        }

        public void Delete()
        {
            Vector2 planPos = _editCursor.TiledHallMousePos;
            _adminEditMode.SetHallPlan(planPos, -1);
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                if (tileDelete && tileDelete.hallContent.combined_pos == $"{planPos.x}_{planPos.y}")
                {
                    Debug.Log("Delete: " + i);
                    _adminEditMode.AddToPosToDelete(new Vector2(tileDelete.hallContent.pos_x,
                        tileDelete.hallContent.pos_z));
                    Destroy(tileDelete.gameObject);
                }
            }
        }
    }
}