using System.Threading;
using UnityEngine;

namespace GenerationMap
{
    public class Point3D
    {
        public float X;
        public float Y;
        public float Z;

        public Point3D()
        {
        }

        public Point3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public static Point3D operator +(Point3D firstPoint3D, Point3D secondPoint3D)
        {
            return new Point3D()
            {
                X = firstPoint3D.X + secondPoint3D.X,
                Y = firstPoint3D.Y + secondPoint3D.Y,
                Z = firstPoint3D.Z + secondPoint3D.Z
            };
        }
    }

    public class GenerationScript : MonoBehaviour
    {
        [SerializeField] public GameObject cube;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateWalls(new Room(Vector3.zero, 10, 5, 5, Vector2.one, new PrefabPack(cube, cube, cube)));
            }
        }

        public void GenerateRoom(Room room)
        {
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
            GenerateRow(ref startPos, length, new Vector3(1, 0, 0), prefabWallChunk);
            GenerateRow(ref startPos, width, new Vector3(0, 0, 1), prefabWallChunk);
            GenerateRow(ref startPos, length, new Vector3(-1, 0, 0), prefabWallChunk);
            GenerateRow(ref startPos, width, new Vector3(0, 0, -1), prefabWallChunk);
        }

        private void GenerateRow(ref Vector3 startPoint, int length, Vector3 direction, GameObject prefab)
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
        }

        private GameObject SpawnChunk(GameObject chunk, Vector3 position)
        {
            return Instantiate(chunk, position, Quaternion.identity);
        }
    }
}