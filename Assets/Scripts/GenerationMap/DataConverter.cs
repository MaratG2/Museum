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
        [SerializeField] public Vector3 positionForSpawn = new Vector3(100,0,100);
        public List<Room> Rooms = new List<Room>();
        private readonly HallQueriesAsync _hallQueriesAsync = new();

        public Vector3 GenerateRoomWithContens(Room roomOptions)
        {
            generationScript.SpawnRoom(roomOptions);
            Rooms.Add(roomOptions);
            return roomOptions.GetSpawnPosition();
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

            return new Room(exhibitsMap, new PrefabPack(wallBlock, floorBlock, cellingBlock), localSpawnPoint,
                positionForSpawn);
        }
        
    }
}
