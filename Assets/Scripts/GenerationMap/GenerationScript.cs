using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GenerationMap
{
    public class GenerationScript : MonoBehaviour
    {
        [SerializeField] public GameObject picture;
        [SerializeField] public GameObject pedestal;

        public void SpawnRoom(Room room)
        {
            var floorBlocs = SpawnPlace(room.PositionRoom, room.Width, room.Length, room.Prefabs.PrefabFloor);
            var height = room.Prefabs.PrefabWall.GetComponent<BoxCollider>().size.y;
            var cellingBlocs = SpawnPlace(room.PositionRoom + new Vector3(0, height, 0), room.Width, room.Length,
                room.Prefabs.PrefabCeiling);
            var wallBlocs = SpawnWalls(room.PositionRoom, room.Width, room.Length, room.Prefabs.PrefabWall,
                room.Prefabs.PrefabFloor);
            room.FloorBlocs = floorBlocs;
            room.WallBlocs = wallBlocs;
            room.CellingBlocs = cellingBlocs;

            room.ExhibitsGO = SpawnExhibits(room);
        }

        private List<GameObject> SpawnExhibits(Room room)
        {
            var exhibits = new List<GameObject>();
            for (var i = 0; i < room.Exhibits.GetLength(0); i++)
            {
                for (var j = 0; j < room.Exhibits.GetLength(1); j++)
                {
                    var exhibitDto = room.Exhibits[i, j];
                    if (exhibitDto.Id == ExhibitsConstants.Picture.Id)
                    {
                        exhibits.Add(SpawnWallExhibit(i, j, exhibitDto, room.WallBlocs, picture));
                        
                    }

                    if (exhibitDto.Id == ExhibitsConstants.Cup.Id)
                    {
                        exhibits.Add(SpawnExhibit(i, j, exhibitDto.HeightSpawn, exhibitDto, room.WallBlocs,
                            room.FloorBlocs, picture));
                        
                    }
                   
                }
            }

            return exhibits;
        }

        private GameObject SpawnWallExhibit(int i, int j, ExhibitDto exhibitDto, GameObject[,] roomWallBlocs,
            GameObject o)
        {
            var nearWall = FindNearWall(i, j, roomWallBlocs);
            var position = nearWall.GetComponent<WallBlock>().SpawnPosition.position;
            var rotate = nearWall.transform.rotation;
            var exhibit = SpawnChunk(picture,
                position, rotate);
            exhibit.GetComponent<ReadPicture>().SetNewPicture(exhibitDto.LinkOnImage);

            return exhibit;
        }

        private GameObject SpawnExhibit(int i, int j, float height, ExhibitDto exhibitDto, GameObject[,] roomWallBlocs,
            GameObject[,] floorBlocks, GameObject o)
        {
            var nearWall = FindNearWall(i, j, roomWallBlocs);
            var rotate = nearWall.transform.rotation;
            var exhibit = SpawnChunk(pedestal,
                floorBlocks[i, j].transform.position + new Vector3(0, height, 0), rotate);
            exhibit.GetComponent<ViewObject>().Name = exhibitDto.TextContentFirst;
            return exhibit;
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

        private GameObject[,] SpawnWalls(Vector3 positionRoom, int roomWidth, int roomLength, GameObject prefabWall,
            GameObject floorPrefab)
        {
            var walls = new GameObject[roomLength, roomWidth];
            var axis = new Vector3(0, 1, 0);
            var scaleFloor = floorPrefab.GetComponent<BoxCollider>().size;
            //вычисление стартовой точки
            positionRoom = new Vector3(positionRoom.x - (float) roomLength / 2 * scaleFloor.x, positionRoom.y,
                scaleFloor.z / 2 + positionRoom.z - (float) roomWidth / 2 * scaleFloor.z);

            //параллельно оси z
            for (int i = 0; i < roomWidth; i++)
            {
                walls[0, i] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(90, axis));
                if (roomWidth - i > 1)
                    positionRoom += new Vector3(0, 0, scaleFloor.z);
            }

            positionRoom += new Vector3(scaleFloor.x / 2, 0, scaleFloor.z / 2);
            //параллельно оси x
            for (var i = 0; i < roomLength; i++)
            {
                walls[i, roomWidth - 1] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(180, axis));
                if (roomLength - i > 1)
                    positionRoom += new Vector3(scaleFloor.x, 0, 0);
            }

            positionRoom += new Vector3(scaleFloor.x / 2, 0, -scaleFloor.z / 2);
            //параллельно оси -z
            for (var i = roomWidth - 1; i >= 0; i--)
            {
                walls[roomLength - 1, i] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(270, axis));
                if (i > 0)
                    positionRoom += new Vector3(0, 0, -scaleFloor.z);
            }

            positionRoom += new Vector3(-scaleFloor.x / 2, 0, -scaleFloor.z / 2);
            //параллельно оси -x
            for (var i = roomLength - 1; i >= 0; i--)
            {
                walls[i, 0] = SpawnChunk(prefabWall, positionRoom, Quaternion.AngleAxis(0, axis));
                if (i > 0)
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
                    blocs[i, j] = SpawnChunk(prefab, tempPos);
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