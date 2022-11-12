using UnityEngine.UI;

namespace GenerationMap
{
    public class ExhibitDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Image Icon { get; set; }
        
        public bool IsBlock { get; set; }
        
        public float HeightSpawn { get; set; }
        public string NameComponent { get; set; }
    }
}