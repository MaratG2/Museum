using UnityEngine;


namespace GenerationMap
{
    public class Exhibit 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector3Int LocalPosition { get; set; }

        public Exhibit( int id, string name, Vector3Int localPosition)
        {
            LocalPosition = localPosition;
            Id = id;
            Name = name;
        }
    }
}