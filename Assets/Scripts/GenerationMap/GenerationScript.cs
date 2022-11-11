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
        public Dictionary<string, GameObject> prefabs;
        

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                var room = new Room(Vector3.zero, 20, 15, 5, Vector2.one, );
                Spawn(room);
            }
        }

        public void SpawnRoom(Room room)
        {
            Spawn(room);
        }

        public void Spawn(Room room)
        {
            for (var i = 0; i < room.Blocks.GetLength(0); i++)
            {
                for (var j = 0; j < room.Blocks.GetLength(1); j++)
                {
                    for (var k = 0; k < room.Blocks.GetLength(2); k++)
                    {
                        GameObject spawnBlock;
                        var kostil = false;

                        spawnBlock = repositoryPrefabs.gameObject.Find(room.Blocks[i, j, k].Name);

                        var localScale = new Vector3(1, 1, 1);
                        if (kostil)
                        {
                            SpawnPicture(room,picture,new Point(i,k),new Vector3(i * localScale.x, j * localScale.y,
                                k * localScale.z));
                            
                            print($"{i}, {k}");
                            continue;
                        }

                        SpawnChunk(spawnBlock,
                            new Vector3(i * localScale.x, j * localScale.y,
                                k * localScale.z));
                    }
                }
            }
        }
        private void SpawnExhibit(Room room, Point internalСoordinates)
        {
            
        }
        private void SpawnPicture(Room room, GameObject picture, Point pos,Vector3 globalPos)
        {
            var rotate = Quaternion.identity;
            var axis = new Vector3(0, 1, 0);
            var deltas = new int[4]
            {
                room.Width - 1 - pos.X,
                pos.X,
                room.Length - 1 - pos.Y,
                pos.Y,
            };
            var minDelta = deltas.Min(x => x);
            if (room.Width - 1 - pos.X == minDelta)
            {
                rotate = Quaternion.AngleAxis(270, axis);
                print("270");
            }
            
            else if (pos.X == minDelta)
            {
                rotate = Quaternion.AngleAxis(90, axis);
                print("90");
            }
            
            else if (room.Length - 1 - pos.Y == minDelta)
            {
                rotate = Quaternion.AngleAxis(180, axis);
                print("180");
            }

            SpawnChunk(picture, globalPos, rotate);
        }

        public void GenerateFloorNew(Room room)
        {
            
        }
        

        public void GenerateRoom(Room room)
        {
        }
        public void GenerateСeiling(Room room)
        {
            var startPosForCelling = room.StartPointRoom +
                                     new Vector3(0, room.Prefabs.PrefabWall.transform.localScale.y * room.Height - 1, 0);
            var deltaWidth = new Vector3(room.Prefabs.PrefabFloor.transform.localScale.x, 0, 0);
            for (var i = 0; i < room.Length; i++)
            {
                GenerateRow(startPosForCelling, room.Width, new Vector3(0,0,1),room.Prefabs.PrefabFloor);
                startPosForCelling += deltaWidth;
            }
        }
        
        public void GenerateFloor(Room room)
        {
            var startPos = room.StartPointRoom;
            var deltaWidth = new Vector3(room.Prefabs.PrefabFloor.transform.localScale.x, 0, 0);
            for (var i = 0; i < room.Length; i++)
            {
                GenerateRow(startPos, room.Width, new Vector3(0,0,1),room.Prefabs.PrefabFloor);
                startPos += deltaWidth;
            }
        }

        public void GenerateWalls(Room room)
        {
            var startPos = room.StartPointRoom;
            var deltaHeight = new Vector3(0, room.Prefabs.PrefabWall.transform.localScale.y, 0);
            for (var i = 0; i < room.Height; i++)
            {
                GenerateStageWalls(startPos, room.Length, room.Width, room.Prefabs.PrefabWall);
                startPos += deltaHeight;
            }
        }


        private void GenerateStageWalls(Vector3 startPosition, int length, int width,
            GameObject prefabWallChunk)
        {
            var startPos = startPosition;
            var tempPosition = GenerateRow(startPos, length, new Vector3(1, 0, 0), prefabWallChunk);
            tempPosition = GenerateRow(tempPosition, width, new Vector3(0, 0, 1), prefabWallChunk);
            tempPosition = GenerateRow(tempPosition, length, new Vector3(-1, 0, 0), prefabWallChunk);
            tempPosition = GenerateRow(tempPosition, width, new Vector3(0, 0, -1), prefabWallChunk);
        }

        private Vector3 GenerateRow(Vector3 startPoint, int length, Vector3 direction, GameObject prefab)
        {
            //var tempPosition = new Vector3(startPoint.x,startPoint.y,startPoint.z);
            var localScale = prefab.transform.localScale;
            var deltaSpawnChunks = new Vector3(direction.x * localScale.x, direction.y * localScale.y,
                direction.z * localScale.z);

            for (var i = 0; i < length; i++)
            {
                SpawnChunk(prefab, startPoint);
                startPoint += deltaSpawnChunks;
            }

            return startPoint;
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