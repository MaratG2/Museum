using System;
using GenerationMap;
using UnityEngine;

public class View3d : MonoBehaviour
{
     [SerializeField]
     private DataConverter dataConverter;
     
     [SerializeField]
     private GenerationConnector generationConnector;

     [SerializeField] private Transform playerTransform;
     private RoomDto _cachedRoomDto;
     private bool _isAllowed;

     private void OnEnable()
     {
          generationConnector.OnDataGot += () =>
          {
               if(_isAllowed) GenerationAfterDelay();
          };
     }
     
     private void OnDisable()
     {
          generationConnector.OnDataGot -= () =>
          {
               if(_isAllowed) GenerationAfterDelay();
          };
     }

     public void GenerationHall(int hnum)
     {
          _isAllowed = true;
          _cachedRoomDto = generationConnector.GetRoomDtoByHnum(hnum);
     }

     private void GenerationAfterDelay()
     {
          _isAllowed = false;
          var room = dataConverter.GetRoomByRoomDto(_cachedRoomDto);
          dataConverter.GenerateRoomWithContens(room);
          var spawnPosition = room.GetSpawnPosition();
          playerTransform.position = spawnPosition;
     }
}
