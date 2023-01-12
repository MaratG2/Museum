using UnityEngine.UI;

namespace Admin.Models
{
    public class ExhibitDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Image Icon { get; set; }
        
        public bool IsBlock { get; set; }
        
        public float HeightSpawn { get; set; }
        public string NameComponent { get; set; }
        public string Description { get; set; }
        
        public string LinkOnImage { get; set; }
        public string TextContentFirst { get; set; }
        public string TextContentSecond { get; set; }
        public System.Drawing.Point LocalPosition { get; set; }
    }
}