using UnityEngine;

namespace GenerationMap
{
    public class PrefabPack
    {
        public readonly GameObject PrefabWall;
        public readonly GameObject PrefabFloor;
        public readonly GameObject PrefabCeiling;
        public readonly GameObject PrefabAngleWall;

        public PrefabPack(GameObject prefabWall, GameObject prefabFloor, GameObject prefabCeiling, GameObject prefabAngleWall)
        {
            PrefabWall = prefabWall;
            PrefabFloor = prefabFloor;
            PrefabCeiling = prefabCeiling;
            PrefabAngleWall = prefabAngleWall;
        }
    }
}
