using UnityEngine;

namespace GenerationMap
{
    public interface IExhibit
    {
        public GameObject Model { get; set; }
        public Vector3 LocalPosition { get; set; }
        
    }
}