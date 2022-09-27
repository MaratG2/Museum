using UnityEngine;

namespace GenerationMap
{
   public class Room
   {
      public readonly Vector3 StartPointRoom;
      public readonly int Length;
      public readonly int Width;
      public readonly int Height;
      //локальная точка спавна 
      public readonly Vector2 LocalSpawnPoint;
      public readonly PrefabPack Prefabs;
      
      public Room(Vector3 startPointRoom, int length, int width, int height, Vector2 localSpawnPoint, PrefabPack prefabPack)
      {
         Length = length;
         Width = width;
         LocalSpawnPoint = CheckLocalSpawnPoint(localSpawnPoint);
         StartPointRoom = startPointRoom;
         Prefabs = prefabPack;
         Height = height;
      }

      public Vector2 GetSpawnPointPosition()
      {
         return new Vector2(StartPointRoom.x + LocalSpawnPoint.x, StartPointRoom.y + LocalSpawnPoint.y);
      }

      private Vector2 CheckLocalSpawnPoint(Vector2 localSpawnPoint)
      {
         var xPos = CheckPosition((int) localSpawnPoint.x, Length);
         var zPos = CheckPosition((int) localSpawnPoint.y, Width);
         return new Vector2(xPos, zPos);
      }

      private int CheckPosition(int delta, int maxDelta)
      {
         if (delta < 1) return 1;
         if (delta > maxDelta - 1) return maxDelta - 1;
         return delta;
      }
   }
}
