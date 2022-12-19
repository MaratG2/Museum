using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Admin.PHP;
using Admin.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace GenerationMap
{
    public class DataConverter : MonoBehaviour
    {
        [SerializeField] private GenerationScript generationScript;
        
        [SerializeField] public GameObject wallBlock;
        [SerializeField] public GameObject floorBlock;
        [SerializeField] public GameObject cellingBlock;
        [SerializeField] public Vector3 positionForSpawn = new(100,0,100);
        private Room _lastSpawnedRoom;

        public void GenerateRoomWithContens(Room roomOptions)
        {
            generationScript.SpawnRoom(roomOptions);
        }
        
        public ExhibitDto GetExhibitByResponse(HallContent content)
        {
            var constExhibit = ExhibitsConstants.GetModelById(content.type);
            return new ExhibitDto()
            {
                Id = constExhibit.Id,
                Name = constExhibit.Name,
                NameComponent = constExhibit.NameComponent,
                HeightSpawn = constExhibit.HeightSpawn,
                IsBlock = constExhibit.IsBlock,
                LocalPosition = new Point(content.pos_x, content.pos_z),
                LinkOnImage = content.image_url,
                TextContentFirst = content.title,

            };
        }

        public Room GetRoomByRoomDto(RoomDto roomDto)
        {
            Debug.Log($"{roomDto.HallOptions.sizex} {roomDto.HallOptions.sizez}");
            var exhibitsData = roomDto.Contents.Select(GetExhibitByResponse).ToList();
            var exhibitsMap = new ExhibitDto[roomDto.HallOptions.sizex, roomDto.HallOptions.sizez];
            
            for (var i = 0; i < exhibitsMap.GetLength(0); i++)
            {
                for (var j = 0; j < exhibitsMap.GetLength(1); j++)
                    exhibitsMap[i, j] = ExhibitsConstants.Floor;
            }

            var localSpawnPoint = new Vector2Int();
            foreach (var exibit in exhibitsData)
            {
                if (exibit.Id == ExhibitsConstants.SpawnPoint.Id)
                    localSpawnPoint = new Vector2Int(exibit.LocalPosition.X, exibit.LocalPosition.Y);
                exhibitsMap[exibit.LocalPosition.X, exibit.LocalPosition.Y] = exibit;
            }

            var tempSpawnPosition = positionForSpawn;

            positionForSpawn *= -1;
            DestroyLastRoom();
            var newRoom = new Room(exhibitsMap, new PrefabPack(wallBlock, floorBlock, cellingBlock), localSpawnPoint,
                tempSpawnPosition);
            _lastSpawnedRoom = newRoom;
            return newRoom;
        }

        private void DestroyLastRoom()
        {
            if (_lastSpawnedRoom == null) return;
            StartCoroutine(generationScript.DestroyRoom(_lastSpawnedRoom));
        }
        
        
    }
}
