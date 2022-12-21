using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Admin.Edit
{
    public class RubberBrush : MonoBehaviour
    {
        public void Rubber()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tiledPos = _tiledMousePos / _tileSize;
                _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][
                    Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] = -1;
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if (tileDelete && tileDelete.hallContent.pos_x == Mathf.FloorToInt(tiledPos.x - _startTilePos.x)
                                   && tileDelete.hallContent.pos_z ==
                                   Mathf.FloorToInt(tiledPos.y - _startTilePos.y))
                    {
                        Debug.Log("Delete: " + i);
                        posToDelete.Add(new Vector2(tileDelete.hallContent.pos_x, tileDelete.hallContent.pos_z));
                        Destroy(tileDelete.gameObject);
                    }
                }
            }
        }
    }
}