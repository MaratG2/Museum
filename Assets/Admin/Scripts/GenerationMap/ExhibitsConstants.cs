using System.Linq;
using System.Reflection;
using Admin.Models;

namespace Admin.GenerationMap
{
    public static class ExhibitsConstants
    {
        public static ExhibitDto SpawnPoint = new()
        {
            Id = 0,
            Icon = null,
            Name = "SpawnPoint",
            IsBlock = false
        };
        
        public static ExhibitDto Picture = new()
        {
            Id = 1,
            Icon = null,
            Name = "Picture",
            IsBlock = false, 
            NameComponent = typeof(PictureBlock).FullName,
            HeightSpawn = 1
        };
        
        public static ExhibitDto InfoBox = new()
        {
            Id = 2,
            Icon = null,
            Name = "InfoBox",
            IsBlock = false
        };
        
        public static ExhibitDto Cup = new()
        {
            Id = 3,
            Icon = null,
            Name = "Cup",
            IsBlock = false,
            HeightSpawn = 1.5f
        };
        
        public static ExhibitDto Medal = new()
        {
            Id = 4,
            Icon = null,
            Name = "Medal",
            IsBlock = false
        };
        
        public static ExhibitDto Video = new()
        {
            Id = 5,
            Icon = null,
            Name = "Video",
            IsBlock = false
        };
        
        public static ExhibitDto Decoration = new()
        {
            Id = 6,
            Icon = null,
            Name = "Decoration",
            IsBlock = false
        };
        
        public static ExhibitDto Floor = new()
        {
            Id = 101,
            Icon = null,
            Name = "Floor",
            IsBlock = true
        };
        
        public static ExhibitDto Celling = new()
        {
            Id = 102,
            Icon = null,
            Name = "Celling",
            IsBlock = true
        };
        
        public static ExhibitDto Wall = new()
        {
            Id = 103,
            Icon = null,
            Name = "Wall",
            IsBlock = true
        };


        public static ExhibitDto GetModelById(int id)
        {
            var models = typeof(ExhibitsConstants)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(x => x.GetValue(null))
                .OfType<ExhibitDto>()
                .Where(x => x.Id == id)
                .ToArray();
            return models.Length == 0 ? null : models.First();
        }
        
        public static ExhibitDto GetModelByName(string name)
        {
            var models = typeof(ExhibitsConstants)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(x => x.GetValue(null))
                .OfType<ExhibitDto>()
                .Where(x => x.Name == name)
                .ToArray();
            return models.Length == 0 ? null : models.First();
        }
    }
}