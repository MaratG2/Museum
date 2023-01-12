using Admin.View;
using GenerationMap;
using UnityEngine;
using UnityEngine.Serialization;

namespace Museum.Scripts.GenerationMap
{
     public class View3d : MonoBehaviour
     {
          [FormerlySerializedAs("generarionConnector")] [SerializeField]
          private GenerationConnector generationConnector;

          [SerializeField] private GameObject player;
          [SerializeField] private GameObject playerCamera;
          [SerializeField] private GameObject mainCanvas;
     
          private HallViewer _hallViewer;
          private TilesDrawer _tilesDrawer;
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

          private void GenerateHall()
          {
               var selectedRoomDto = GetSelectedRoomDto();
          
               var selectedRoom= generationConnector.GetRoomByRoomDto(selectedRoomDto);
               generationConnector.GenerateRoomWithContens(selectedRoom);
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
}
