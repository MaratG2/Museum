using UnityEngine;

namespace GenerationMap
{
    public class PrefabPack
    {
        public readonly GameObject PrefabWall;
        public readonly GameObject PrefabFloor;
        public readonly GameObject PrefabCeiling;

        public PrefabPack(GameObject prefabWall, GameObject prefabFloor, GameObject prefabCeiling)
        {
            PrefabWall = prefabWall;
            PrefabFloor = prefabFloor;
            PrefabCeiling = prefabCeiling;
        }
    }
}
