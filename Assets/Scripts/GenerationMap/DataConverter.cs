using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public void Start()
        {
            ConnectionDb.OpenConnection();
        }

        public Vector3 GenerateRoomByOnum(int num)
        {
            return GenerateRoomByOnum(ConnectionDb.GetOptionByOnum(num));
        }

        public Vector3 GenerateRoomByOnum(Hall roomOptions)
        {
            var room = GetRoomByResponse(roomOptions);
            generationScript.SpawnRoom(room);
            Rooms.Add(room);
            return room.GetSpawnPosition();
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
        
        public Room GetRoomByResponse(Hall room)
        {
            var exhibitsData = ConnectionDb
                .GetAllContentByOnum(room.hnum)
                .Select(GetExhibitByResponse)
                .ToList();
            Debug.Log($"{exhibitsData.Count}");


            var exhibitsMap = new ExhibitDto[room.sizex, room.sizez];
            for (var i = 0; i < exhibitsMap.GetLength(0); i++)
            {
                for (var j = 0; j < exhibitsMap.GetLength(1); j++)
                {
                    exhibitsMap[i, j] = ExhibitsConstants.Floor;
                }
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
