using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using GenerationMap;
using Unity.VisualScripting;
using UnityEngine;

namespace Admin.Edit
{
    public class PaintBrush : MonoBehaviour
    {
        private ToolSelector _toolSelector;
        
        private void Awake()
        {
            _toolSelector = GetComponent<ToolSelector>();
        }

        private void Update()
        {
            BlockDoorToolIfNeeded();
            if (_toolSelector.CanDraw() && Input.GetMouseButtonDown(0))
                Paint(_tiledMousePos / _tileSize, _cursorTile.anchoredPosition);
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
        private void Paint(Vector2 tiledPos, Vector2 pos, bool hasStruct = false, HallContent content = new())
        {
            if (_hallPlan == null)
                return;
            if (Mathf.FloorToInt(tiledPos.x - _startTilePos.x) >= _hallPlan.Length ||
                (Mathf.FloorToInt(tiledPos.x - _startTilePos.x) < _hallPlan.Length
                 && Mathf.FloorToInt(tiledPos.y - _startTilePos.y) >=
                 _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)].Length))
            {
                return;
            }

            var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity,
                _paintsParent);
            newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
            newTile.GetComponent<RectTransform>().anchoredPosition = pos;
            newTile.GetComponent<Image>().color = _cursorTile.GetComponent<Image>().color;

            _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] =
                _currentTool;
            Tile tileInstance = newTile.GetComponent<Tile>();
            tileInstance.hallContent.type = _currentTool;
            tileInstance.hallContent.pos_x = Mathf.FloorToInt(tiledPos.x - _startTilePos.x);
            tileInstance.hallContent.pos_z = Mathf.FloorToInt(tiledPos.y - _startTilePos.y);
            tileInstance.hallContent.combined_pos =
                tileInstance.hallContent.pos_x + "_" + tileInstance.hallContent.pos_z;

            if (tileInstance.hallContent.type == ExhibitsConstants.Picture.Id
                || tileInstance.hallContent.type == ExhibitsConstants.Video.Id)
            {
                tileInstance.hallContent.image_desc = "desc";
                tileInstance.hallContent.image_url = "url";
                tileInstance.hallContent.title = "title";
            }

            if (tileInstance.hallContent.type == ExhibitsConstants.Decoration.Id)
            {
                tileInstance.hallContent.title = _decorationsDropdown.value.ToString();
                tileInstance.hallContent.image_url = "Decoration";
                tileInstance.hallContent.image_desc = _decorationsDropdown.options[_decorationsDropdown.value].text;
            }

            for (int i = 0; i < posToDelete.Count; i++)
            {
                if (tileInstance.hallContent.combined_pos == posToDelete[i].x + "_" + posToDelete[i].y)
                    posToDelete.RemoveAt(i);
            }

            //tileInstance.hallContent.uid = uid;
            if (hasStruct)
            {
                tileInstance.hallContent = content;
            }

            tileInstance.Setup();
            if (tileInstance.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
            {
                SelectTool(-3);
                _isDoorBlock = true;
                _tileSelected = null;
            }
        }
    }
}