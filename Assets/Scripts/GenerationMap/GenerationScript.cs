using UnityEngine;

namespace GenerationMap
{
    public class GenerationScript : MonoBehaviour
    {

        public void GenerateRoom(Room room)
        {
            
        }

        private void GenerateWalls(Room room)
        {
            var startPos = room.StartPointRoom;
            var localScale = room.Prefabs.PrefabWall.transform.localScale;
            var deltaX = localScale.x;
            var deltaY = localScale.y;
            var deltaZ = localScale.z;
            
            for (int i = 0; i < room.Length; i++)
            {
                SpawnChunk(room.Prefabs.PrefabWall, startPos);
                startPos.x += deltaX;
            }
            
            for (int i = 0; i < room.Width; i++)
            {
                SpawnChunk(room.Prefabs.PrefabWall, startPos);
                startPos.z += deltaZ;
            }
            
            for (int i = 0; i < room.Length; i++)
            {
                SpawnChunk(room.Prefabs.PrefabWall, startPos);
                startPos.x -= deltaX;
            }
            
            for (int i = 0; i < room.Width; i++)
            { 
                SpawnChunk(room.Prefabs.PrefabWall, startPos);
                startPos.z -= deltaZ;
            }
        }

        private GameObject SpawnChunk(GameObject chunk, Vector3 position)
        {
            return Instantiate(chunk, position, Quaternion.identity);
        }
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
