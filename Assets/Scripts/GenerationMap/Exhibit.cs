using UnityEngine;

namespace GenerationMap
{
    public class Exhibit : IExhibit
    {
        public GameObject Model { get; set; }
        public Vector3 LocalPosition { get; set; }

        public Exhibit(GameObject model, Vector3 localPosition)
        {
            Model = model;
            LocalPosition = localPosition;
        }
    }
}