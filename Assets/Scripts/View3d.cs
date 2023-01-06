using GenerationMap;
using UnityEngine;

public class View3d : MonoBehaviour
{
     [SerializeField]
     private DataConverter dataConverter;
     
     [SerializeField]
     private GenerationConnector generationConnector;

     [SerializeField] private Transform playerTransform;

     public void GenerationHall(int hnum)
     {
          var roomDto = generationConnector.GetRoomDtoByHnum(hnum);
          var room = dataConverter.GetRoomByRoomDto(roomDto);
          dataConverter.GenerateRoomWithContens(room);
          var spawnPosition = room.GetSpawnPosition();
          playerTransform.position = spawnPosition;
     }

}
