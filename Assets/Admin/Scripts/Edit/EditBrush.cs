using System;
using System.Collections;
using System.Collections.Generic;
using GenerationMap;
using UnityEngine;

namespace Admin.Edit
{
    public class EditBrush : MonoBehaviour
    {
        [SerializeField] private Transform _paintsParent;
        public Tile TileSelected { get; private set; }
        private EditCursor _editCursor;

        private void Awake()
        {
            _editCursor = GetComponent<EditCursor>();
        }
        
        public void Edit()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tileRealPos = _editCursor.TiledHallMousePos;
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if (tileChange && tileChange.hallContent.pos_x == tileRealPos.x &&
                        tileChange.hallContent.pos_z == tileRealPos.y)
                    {
                        if (tileChange.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
                        {
                            Debug.Log("Tile Change Door: " + i);
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.Picture.Id)
                        {
                            _propertiesHeader.text = "Редактирование фото";
                            _isCursorLock = true;
                            _changePropertiesGroup.SetActive(true);
                            _photoVideoGroup.SetActive(true);
                            _propertiesName.text = tileChange.hallContent.title;
                            _propertiesUrl.text = tileChange.hallContent.image_url;
                            _propertiesDesc.text = tileChange.hallContent.image_desc;
                            TileSelected = tileChange;
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.InfoBox.Id)
                        {
                            _isCursorLock = true;
                            _changePropertiesGroup.SetActive(true);
                            _infoGroup.SetActive(true);
                            _infoBoxName.text = tileChange.hallContent.title;
                            _infoController.Setup(tileChange.hallContent.image_desc);
                            TileSelected = tileChange;
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.Cup.Id)
                        {
                            Debug.Log("Tile Change Cup: " + i);
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.Medal.Id)
                        {
                            Debug.Log("Tile Change Medal: " + i);
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.Video.Id)
                        {
                            _propertiesHeader.text = "Редактирование видео";
                            _isCursorLock = true;
                            _changePropertiesGroup.SetActive(true);
                            _photoVideoGroup.SetActive(true);
                            _propertiesName.text = tileChange.hallContent.title;
                            _propertiesUrl.text = tileChange.hallContent.image_url;
                            _propertiesDesc.text = tileChange.hallContent.image_desc;
                            TileSelected = tileChange;
                        }

                        if (tileChange.hallContent.type == ExhibitsConstants.Decoration.Id)
                        {
                            _decorationsDropdown.value = Int32.Parse(tileChange.hallContent.title);
                            _isCursorLock = true;
                            _changePropertiesGroup.SetActive(true);
                            _decorGroup.SetActive(true);
                            TileSelected = tileChange;
                        }
                    }
                }
            }
        }
    }
}