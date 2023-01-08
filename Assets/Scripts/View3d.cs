using System;
using Admin.Utility;
using Admin.View;
using GenerationMap;
using UnityEngine;

public class View3d : MonoBehaviour
{
     [SerializeField]
     private DataConverter dataConverter;
     
     /*[SerializeField]
     private GenerationConnector generationConnector;*/

     [SerializeField] private GameObject player;
     [SerializeField] private GameObject playerCamera;
     [SerializeField] private GameObject mainCanvas;
     private HallViewer _hallViewer;
     private TilesDrawer _tilesDrawer;
     private RoomDto _cachedRoomDto;
     private bool _isView;

     public void Awake()
     {
          _hallViewer = FindObjectOfType<HallViewer>();
          _tilesDrawer = FindObjectOfType<TilesDrawer>();
          
          player.SetActive(false);
          playerCamera.SetActive(false);
     }

     public void Update()
     {
          if (Input.GetKeyDown(KeyCode.R))
               Exit3DView();
     }

     public void To3DView()
     {
          if (_isView)
               return;
          _isView = true;
          State.SetCursorLock();
          
          mainCanvas.SetActive(false);
          player.SetActive(true);
          playerCamera.SetActive(true);
          
          GenerateHall();
     }
     
     public void Exit3DView()
     {
          if (!_isView)
               return;
          
          _isView = false;
          State.View();
          
          mainCanvas.SetActive(true);
          player.SetActive(false);
          playerCamera.SetActive(false);
     }

     public void GenerateHall()
     {
          var selectedRoomDto = GetSelectedRoomDto();
          
          var selectedRoom= dataConverter.GetRoomByRoomDto(selectedRoomDto);
          dataConverter.GenerateRoomWithContens(selectedRoom);
          var spawnPosition = selectedRoom.GetSpawnPosition();
          
          player.transform.position = spawnPosition;
     }

     private RoomDto GetSelectedRoomDto()
     {
          return new RoomDto()
          {
               HallOptions = _hallViewer.HallSelected,
               Contents = _tilesDrawer.HallContents,
          };
     }
}
