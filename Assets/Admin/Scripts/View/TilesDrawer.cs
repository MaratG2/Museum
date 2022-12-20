using System;
using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using UnityEngine;

namespace Admin.View
{
    public class TilesDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _tilesParent;
        [SerializeField] private GameObject _hallPreview;
        private HallQueries _hallQueries = new ();
        private float _tileSize;
        private Vector2 _leftBottomTilePos;
        
        private void OnEnable()
        {
            _hallQueries.OnAllHallContentsGet += DrawTilesForGotHall;
        }

        private void OnDisable()
        {
            _hallQueries.OnAllHallContentsGet -= DrawTilesForGotHall;
        }

        public void SetPreviewState(bool setTo)
        {
            _hallPreview.SetActive(setTo);
        }

        public void ClearAllTiles()
        {
            for (int i = 0; i < _tilesParent.transform.childCount; i++)
                Destroy(_tilesParent.transform.GetChild(i).gameObject);
        }
        
        public IEnumerator DrawTilesForHall(Hall hall)
        {
            SetPreviewState(true);
            yield return new WaitForSecondsRealtime(0.1f);
            yield return CalculateTileSize(hall.sizex);
            yield return FindLeftBottomTilePosition();
            yield return _hallQueries.GetAllContentsByHnum(hall.hnum);
        }
        
        private void DrawTilesForGotHall(List<HallContent> hallContents)
        {
            if (hallContents == null)
                return;
            
            foreach (var hallContent in hallContents)
            {
                if (hallContent.hnum == 0)
                    continue;

                Vector2 contentPos = new Vector2(hallContent.pos_x, hallContent.pos_z);
                Vector2 tilePos = _leftBottomTilePos + contentPos;
                Vector2 drawPos = new Vector2
                (
                    tilePos.x * _tileSize,
                    tilePos.y * _tileSize
                );
                Paint(drawPos, hallContent);
            }
        }
        
        private void Paint(Vector2 pos, HallContent content)
        {
            var newTileGO = Instantiate(_tilePrefab.gameObject, Vector2.zero, Quaternion.identity,
                _tilesParent.GetComponent<RectTransform>());
            var newTileRT = newTileGO.GetComponent<RectTransform>();
            newTileRT.anchorMin = Vector2.zero;
            newTileRT.anchorMax = Vector2.zero;
            newTileRT.anchoredPosition = pos;
            newTileRT.sizeDelta = new Vector2(_tileSize, _tileSize);
            var newTile = newTileGO.GetComponent<Tile>();
            newTile.GetComponent<Tile>().hallContent = content;
            newTile.GetComponent<Tile>().Setup();
        }

        private IEnumerator CalculateTileSize(int hallSizeX)
        {
            do
            {
                _tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / hallSizeX;
                yield return new WaitForEndOfFrame();
            } while (_tileSize.Equals(0f));
        }

        private IEnumerator FindLeftBottomTilePosition()
        {
            for (int i = 0; i < 1920 / _tileSize; i++)
                for (int j = 0; j < 1080 / _tileSize; j++)
                    if (CheckIfTileExistsAt(i, j))
                    {
                        _leftBottomTilePos = new Vector2
                        (
                            i + 0.5f,
                            j + 0.25f
                        );
                        yield break;
                    }
        }

        private bool CheckIfTileExistsAt(int x, int y)
        {
            bool isOverPreview = false;
            GameObject[] casted = RaycastUtilities.UIRaycasts(
                RaycastUtilities.ScreenPosToPointerData(
                    new Vector2(x * _tileSize + _tileSize / 2, y * _tileSize + _tileSize / 4)));
            foreach (var c in casted)
            {
                if (c.GetComponent<HallPreviewResizer>())
                    isOverPreview = true;
            }

            return isOverPreview;
        }
    }
}