using Admin.View;
using GenerationMap;
using InProject;
using UnityEngine;
using UnityEngine.Serialization;

namespace Museum.Scripts.GenerationMap
{
     /// <summary>
     /// Отвечает за загрузку данных о выбранном зале в администраторском приложении и дальнейшей передаче его для генерации в GenerationConnector.
     /// </summary>
     /// <remarks>
     /// Используется только в приложении для администраторов.
     /// </remarks>
     public class View3d : MonoBehaviour
     {
          [FormerlySerializedAs("generarionConnector")] [SerializeField]
          private GenerationConnector generationConnector;

          [SerializeField] private GameObject player;
          [SerializeField] private GameObject playerCamera;
          [SerializeField] private GameObject mainCanvas;
          [SerializeField] private TilesDrawer _tilesDrawerView;
          [SerializeField] private TilesDrawer _tilesDrawerEdit;
          
          private HallViewer _hallViewer;
          private bool _isView;

          public void Awake()
          {
               _hallViewer = FindObjectOfType<HallViewer>();
          
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
               State.SetCursorUnlock();
          
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
                    Contents = _tilesDrawerView.HallContents.Count != 0 ?
                         _tilesDrawerView.HallContents : _tilesDrawerEdit.HallContents
               };
          }
     }
}
