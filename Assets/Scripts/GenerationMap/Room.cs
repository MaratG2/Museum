using System.Collections.Generic;
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
      public List<Exhibit> Exhibits;
      public TypeBlock[,,] Blocks;

      public Room(Vector3 startPointRoom, int length, int width, int height, Vector2 localSpawnPoint, PrefabPack prefabPack, List<Exhibit> exhibits)
      {
         Length = length;
         Width = width;
         LocalSpawnPoint = CheckLocalSpawnPoint(localSpawnPoint);
         StartPointRoom = startPointRoom;
         Prefabs = prefabPack;
         Height = height;
         Exhibits = exhibits;
         Blocks = GenerateMap(width, length, height);
      }
      
      public TypeBlock[,,] GenerateMap(int width, int length, int height)
      {
         var resultMap = new TypeBlock[width, height, length];
         for (var i = 0; i < length; i++)
         {
            for (var j = 0; j < width; j++)
            {
               resultMap[j, 0, i] = TypeBlock.Floor;
               resultMap[j, height - 1, i] = TypeBlock.Celling;
            }
         }

         for (int i = 0; i < width; i++)
         {
            for (int j = 0; j < height; j++)
            {
               resultMap[i, j, 0] = TypeBlock.Wall;
               resultMap[i, j, length - 1] = TypeBlock.Wall;
            }
         }
            
         for (int i = 0; i < length; i++)
         {
             for (int j = 0; j < height; j++)
             {
                 resultMap[0, j, i] = TypeBlock.Wall;
                 resultMap[width - 1, j, i] = TypeBlock.Wall;
             }
         }

         foreach (var exhibit in Exhibits)
         {
            resultMap[(int) exhibit.LocalPosition.x, (int) exhibit.LocalPosition.y, (int) exhibit.LocalPosition.z] = TypeBlock.Exhibit;
         }

         return resultMap;
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
