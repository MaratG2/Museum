using System.Collections;
using GenerationMap;
using UnityEngine;

namespace Museum.Scripts.GenerationMap
{
    /// <summary>
    /// Отвечает за генерацию стен, полов и потолков зала, а также создание карты экспонатов.
    /// </summary>
    public class GenerationScript : MonoBehaviour
    {
        [SerializeField] private ExhibitsSpawner exhibitsSpawner;
        public IEnumerator DestroyRoom(Room room)
        {
            foreach (var exhibit in room.ExhibitsGO)
                Destroy(exhibit);
            foreach (var floor in room.FloorBlocs)
                Destroy(floor);
            foreach (var cell in room.CellingBlocs)
                Destroy(cell);
            foreach (var wall in room.WallBlocs)
                Destroy(wall);
            yield return null;
        }

        public void SpawnRoom(Room room)
        {
            var floorBlocs = SpawnPlace(room.PositionRoom, room.Width, room.Length, room.Prefabs.PrefabFloor);
            var height = room.Prefabs.PrefabWall.GetComponent<BoxCollider>().size.y;
            
            var cellingBlocs = SpawnPlace(room.PositionRoom + new Vector3(0, height, 0), room.Width, room.Length,
                room.Prefabs.PrefabCeiling);
            
            var wallBlocs = SpawnWallsV2(room.PositionRoom, room.Width, room.Length, room);
            room.FloorBlocs = floorBlocs;
            room.WallBlocs = wallBlocs;
            room.CellingBlocs = cellingBlocs;

            room.ExhibitsGO = exhibitsSpawner.SpawnExhibits(room);
        }

        private GameObject[,] SpawnWallsV2(Vector3 positionRoom, int roomWidth, int roomLength, Room room)
        {
            var matrixWalls = new GameObject[roomLength, roomWidth];
            var axis = new Vector3(0, 1, 0);
            var tempPosition = GetStartPositionForSpawnWall(positionRoom, room);
            
            var prefabWall = room.Prefabs.PrefabWall;
            var prefabAngleWall = room.Prefabs.PrefabAngleWall;
            var scaleFloor = room.Prefabs.PrefabFloor.GetComponent<BoxCollider>().size;
            tempPosition += new Vector3(0, 0, scaleFloor.z);
            
            //паралельно оси Z
            for (var i = 1; i < roomWidth - 1; i++)
            {
                matrixWalls[0, i] = SpawnChunk(prefabWall, tempPosition, Quaternion.AngleAxis(90, axis));
                tempPosition += new Vector3(0, 0, scaleFloor.z);
            }
            
            //установка углового блока 
            tempPosition += new Vector3(scaleFloor.x / 2, 0, 0);
            matrixWalls[0, roomWidth - 1] = SpawnChunk(prefabAngleWall, tempPosition, Quaternion.AngleAxis(90, axis));
            tempPosition += new Vector3(scaleFloor.x , 0, scaleFloor.z / 2);

            //паралельно оси X
            for (var i = 1; i < roomLength - 1; i++)
            {
                matrixWalls[i, roomWidth - 1] = SpawnChunk(prefabWall, tempPosition, Quaternion.AngleAxis(180, axis));
                tempPosition += new Vector3(scaleFloor.x, 0, 0);
            }
            
            //установка углового блока 
            tempPosition += new Vector3(0, 0, -scaleFloor.z / 2);
            matrixWalls[roomLength - 1, roomWidth - 1] = SpawnChunk(prefabAngleWall, tempPosition, Quaternion.AngleAxis(180, axis));
            tempPosition += new Vector3(scaleFloor.x / 2, 0, -scaleFloor.z);
            
            //паралельно оси -Z
            for (var i = roomWidth - 2; i > 0; i--)
            {
                matrixWalls[roomLength - 1, i] = SpawnChunk(prefabWall, tempPosition, Quaternion.AngleAxis(270, axis));
                tempPosition += new Vector3(0, 0, -scaleFloor.z);
            }
            
            //установка углового блока 
            tempPosition += new Vector3(-scaleFloor.x / 2, 0, 0);
            matrixWalls[roomLength - 1, 0] = SpawnChunk(prefabAngleWall, tempPosition, Quaternion.AngleAxis(270, axis));
            tempPosition += new Vector3(-scaleFloor.x, 0, -scaleFloor.z / 2);
            
            //паралельно оси -x
            for (var i = roomLength - 2; i > 0; i--)
            {
                matrixWalls[i, 0] = SpawnChunk(prefabWall, tempPosition, Quaternion.AngleAxis(0, axis));
                tempPosition += new Vector3(-scaleFloor.x, 0, 0);
            }
            
            //установка углового блока 
            tempPosition += new Vector3(0, 0, scaleFloor.z / 2);
            matrixWalls[0, 0] = SpawnChunk(prefabAngleWall, tempPosition, Quaternion.AngleAxis(0, axis));
            
            return matrixWalls;
        }

        private Vector3 GetStartPositionForSpawnWall(Vector3 positionRoom, Room room)
        {
            var scaleFloor = room.Prefabs.PrefabFloor.GetComponent<BoxCollider>().size;
            return new Vector3(positionRoom.x - (float) room.Length / 2 * scaleFloor.x, positionRoom.y,
                scaleFloor.z / 2 + positionRoom.z - (float) room.Width / 2 * scaleFloor.z);
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