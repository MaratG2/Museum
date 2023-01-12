using System.Collections.Generic;
using System.Linq;
using Admin.Models;
using UnityEngine;

namespace GenerationMap
{
    public class Room
    {
        public ExhibitDto[,] Exhibits { get; }
        public readonly int Length;
        public readonly int Width;
        public readonly PrefabPack Prefabs;
        public readonly Vector2Int LocalSpawnPoint;
        public readonly Vector3 PositionRoom;
        public GameObject[,] FloorBlocs;
        public GameObject[,] WallBlocs;
        public GameObject[,] CellingBlocs;
        public List<GameObject> ExhibitsGO;

        public Room(ExhibitDto[,] exhibits, PrefabPack prefabs, Vector2Int localSpawnPoint, Vector3 positionRoom)
        {
            Exhibits = exhibits;
            Prefabs = prefabs;
            LocalSpawnPoint = localSpawnPoint;
            PositionRoom = positionRoom;

            Width = exhibits.GetLength(1);

            Length = exhibits.GetLength(0);
        }

        public Vector3 GetSpawnPosition()
        {
            if (LocalSpawnPoint == Vector2Int.zero) return PositionRoom + new Vector3(0, 0.2f, 0);
            var scale = Prefabs.PrefabFloor.GetComponent<BoxCollider>().size;
            var startPoint = new Vector3(PositionRoom.x - (float) Length / 2 * scale.x, PositionRoom.y,
                scale.z / 2 + PositionRoom.z - (float) Width / 2 * scale.z);
            startPoint += new Vector3(scale.x * LocalSpawnPoint.x, 1, scale.z * LocalSpawnPoint.y);
            return startPoint;
        }
    }
}