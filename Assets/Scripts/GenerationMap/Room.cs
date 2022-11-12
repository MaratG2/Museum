using System.Collections.Generic;
using System.Linq;
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
      public List<GameObject> Objects;

      public Room(ExhibitDto[,] exhibits, PrefabPack prefabs, Vector2Int localSpawnPoint, Vector3 positionRoom)
      {
         Exhibits = exhibits;
         Prefabs = prefabs;
         LocalSpawnPoint = localSpawnPoint;
         PositionRoom = positionRoom;
         
         Width = exhibits.GetLength(1);
         
         Length = exhibits.GetLength(0);
      }
   }
}
