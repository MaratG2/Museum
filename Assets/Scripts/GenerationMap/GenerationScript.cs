using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace GenerationMap
{
    public class GenerationScript : MonoBehaviour
    {
        [SerializeField] public GameObject floor;
        [SerializeField] public GameObject wall;
        [SerializeField] public GameObject celling;
        [SerializeField] public GameObject picture;
        [SerializeField] public GameObject repositoryPrefabs;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var map = new ExhibitDto[5,11];
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = ExhibitsConstants.Picture;
                        
                    }
                }

                var prPack = new PrefabPack(wall, floor, celling);
                var room = new Room(map, prPack, new Vector2Int(2, 2), Vector3.zero);
                SpawnRoom(room);
            }
        }

        public void SpawnRoom(Room room)
        {
            var floorBlocs = SpawnPlace(room.PositionRoom, room.Width, room.Length, room.Prefabs.PrefabFloor);
            var height = room.Prefabs.PrefabWall.GetComponent<BoxCollider>().size.y;
            var cellingBlocs = SpawnPlace(room.PositionRoom + new Vector3(0,height,0), room.Width, room.Length, room.Prefabs.PrefabCeiling);
            var wallBlocs = SpawnWalls(room.PositionRoom, room.Width, room.Length, room.Prefabs.PrefabWall,
                room.Prefabs.PrefabFloor);
            room.FloorBlocs = floorBlocs;
            room.WallBlocs = wallBlocs;
            room.CellingBlocs = cellingBlocs;

            SpawnExhibits(room);
        }

        private void SpawnExhibits(Room room)
        {
            for (int i = 0; i < room.Exhibits.GetLength(0); i++)
            {
                for (int j = 0; j < room.Exhibits.GetLength(1); j++)
                {
                    var exhibitDto = room.Exhibits[i, j];
                    if (exhibitDto.NameComponent == "PictureBlock")
                    {
                        SpawnWallExhibit(i, j, room.WallBlocs, picture);
                    }
                    
                    if (exhibitDto.NameComponent == "Pedestal")
                    {
                        SpawnExhibit(i, j, exhibitDto.HeightSpawn, room.WallBlocs, room.FloorBlocs, picture);
                    }
                }
            }
        }

        private void SpawnWallExhibit(int i, int j, GameObject[,] roomWallBlocs, GameObject o)
        {
            var nearWall = FindNearWall(i, j, roomWallBlocs);
            var position = nearWall.GetComponent<WallBlock>().SpawnPosition.position;
            var rotate = nearWall.transform.rotation;
            SpawnChunk(picture,
                position, rotate);
        }
        
        private void SpawnExhibit(int i, int j, float height, GameObject[,] roomWallBlocs, GameObject[,] floorBlocks, GameObject o)
        {
            var nearWall = FindNearWall(i, j, roomWallBlocs);
            var rotate = nearWall.transform.rotation;
            SpawnChunk(picture,
                floorBlocks[i,j].transform.position + new Vector3(0,height, 0), rotate);
        }


        private GameObject FindNearWall(int i, int j, GameObject[,] roomWallBlocs)
        {
            if (i == 1) return roomWallBlocs[0, j];
            if (j == 1) return roomWallBlocs[i, 0];
            if (i == roomWallBlocs.GetLength(0) - 2) return roomWallBlocs[roomWallBlocs.GetLength(0) - 1, j];
            if (j == roomWallBlocs.GetLength(1) - 2) return roomWallBlocs[i, roomWallBlocs.GetLength(1) - 1];

            if (i < roomWallBlocs.GetLength(0) / 2)
                return roomWallBlocs[0, j];
            return roomWallBlocs[roomWallBlocs.GetLength(0) - 1, j];
        }

        private GameObject[,] SpawnWalls(Vector3 positionRoom, int roomWidth, int roomLength, GameObject prefabWall, GameObject floorPrefab)
        {
            var walls = new GameObject[roomLength, roomWidth];
            var axis = new Vector3(0, 1, 0);
            var scaleFloor = floorPrefab.GetComponent<BoxCollider>().size;
            //вычисление стартовой точки
            positionRoom = new Vector3( positionRoom.x - (float) roomLength / 2 * scaleFloor.x, positionRoom.y,
                scaleFloor.z / 2 +positionRoom.z - (float) roomWidth / 2 * scaleFloor.z);
            
            //параллельно оси z
            for (int i = 0; i < roomWidth; i++)
            {
                walls[0,i] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(90, axis));
                if(roomWidth - i > 1)
                    positionRoom += new Vector3(0, 0, scaleFloor.z);
            }
            
            positionRoom += new Vector3(scaleFloor.x / 2, 0, scaleFloor.z / 2);
            //параллельно оси x
            for (var i = 0; i < roomLength; i++)
            {
                walls[i,roomWidth - 1] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(180, axis));
                if(roomLength - i > 1)
                    positionRoom += new Vector3(scaleFloor.x, 0, 0);
            }
            
            positionRoom += new Vector3(scaleFloor.x / 2, 0, -scaleFloor.z / 2);
            //параллельно оси -z
            for (var i = roomWidth - 1; i >= 0; i--)
            {
                walls[roomLength - 1, i] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(270, axis));
                if(i > 0)
                    positionRoom += new Vector3(0, 0, -scaleFloor.z);
            }
            
            positionRoom += new Vector3(-scaleFloor.x / 2, 0, -scaleFloor.z / 2);
            //параллельно оси -x
            for (var i = roomLength - 1; i >= 0; i--)
            {
                walls[i, 0] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(0, axis));
                if(i > 0)
                    positionRoom += new Vector3(-scaleFloor.x, 0, 0);
            }

            return walls;
        }

        private GameObject[,] SpawnPlace(Vector3 positionRoom, int roomWidth, int roomLength, GameObject prefab)
        {
            var blocs = new GameObject[roomLength, roomWidth];
            var scale = prefab.GetComponent<BoxCollider>().size;
            //находим крайнюю точку исзодя из центра комнаты
            positionRoom = new Vector3(scale.x / 2 + positionRoom.x - (float) roomLength / 2 * scale.x, positionRoom.y,
                scale.x / 2 + positionRoom.z - (float) roomWidth / 2 * scale.z);
            for (int i = 0; i < blocs.GetLength(0); i++)
            {
                for (int j = 0; j < blocs.GetLength(1); j++)
                {
                    var tempPos = new Vector3(positionRoom.x + i * scale.x, positionRoom.y,
                        positionRoom.z + j * scale.z);
                    blocs[i,j] = SpawnChunk(prefab, tempPos);
                }
            }
            return blocs;
        }
       
        private GameObject SpawnChunk(GameObject chunk, Vector3 position, Quaternion rotate)
        {
            return Instantiate(chunk, position, rotate);
        }
        private GameObject SpawnChunk(GameObject chunk, Vector3 position)
        {
            return Instantiate(chunk, position, Quaternion.identity);
        }
    }
}