using System.Collections.Generic;
using System.Linq;
using Admin.Utility;
using UnityEngine;

namespace GenerationMap
{
    public class GenerationConnector : MonoBehaviour
    {
        [SerializeField] private GenerationScript generationScript;
        
        [SerializeField] public GameObject wallBlock;
        [SerializeField] public GameObject floorBlock;
        [SerializeField] public GameObject cellingBlock;
        [SerializeField] public GameObject angleWallBlock;
        [SerializeField] public Vector3 positionForSpawn = new(100, 0, 100);
        private Room _lastSpawnedRoom;

        public void GenerateRoomWithContens(Room roomOptions)
        {
            generationScript.SpawnRoom(roomOptions);
        }

        public Room GetRoomByRoomDto(RoomDto roomDto)
        {
            var exhibitsData = roomDto.Contents
                .Select(x => x.ToExhibitDto())
                .ToList();

            var exhibitsMap = GetExhibitsMap(roomDto, exhibitsData);
            
            var tempSpawnPosition = positionForSpawn;

            positionForSpawn *= -1;
            DestroyLastRoom();
            var newRoom = new Room(exhibitsMap, new PrefabPack(wallBlock, floorBlock, cellingBlock, angleWallBlock),
                GetLocalSpawnPosition(roomDto, exhibitsData),
                tempSpawnPosition);
            
            _lastSpawnedRoom = newRoom;
            return newRoom;
        }
        
        private Vector2Int GetLocalSpawnPosition(RoomDto roomDto, List<ExhibitDto> exhibits)
        {
            var localSpawnPoint = new Vector2Int(roomDto.HallOptions.sizex / 2, roomDto.HallOptions.sizez / 2);
            
            var spawnPoints = exhibits
                .Where(x => x.Id == ExhibitsConstants.SpawnPoint.Id)
                .ToList();
            
            if (spawnPoints.Count != 0)
                localSpawnPoint = new Vector2Int(spawnPoints.First().LocalPosition.X,
                    spawnPoints.First().LocalPosition.X);
            return localSpawnPoint;
        }

        private ExhibitDto[,] GetExhibitsMap(RoomDto room, List<ExhibitDto> exhibits)
        {
            var exhibitsMap = new ExhibitDto[room.HallOptions.sizex, room.HallOptions.sizez];

            for (var i = 0; i < exhibitsMap.GetLength(0); i++)
                for (var j = 0; j < exhibitsMap.GetLength(1); j++)
                    exhibitsMap[i, j] = ExhibitsConstants.Floor;
            
            foreach (var exibit in exhibits)
                exhibitsMap[exibit.LocalPosition.X, exibit.LocalPosition.Y] = exibit;
            return exhibitsMap;
        }

        private void DestroyLastRoom()
        {
            if (_lastSpawnedRoom == null) return;
            StartCoroutine(generationScript.DestroyRoom(_lastSpawnedRoom));
        }
        
        
    }
}
