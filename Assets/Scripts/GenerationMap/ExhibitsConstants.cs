using System.Linq;
using System.Reflection;

namespace GenerationMap
{
    public static class ExhibitsConstants
    {
        public static ExhibitDto Picture = new ExhibitDto()
        {
            Id = 1,
            Icon = null,
            Name = "Picture",
            IsBlock = false, 
            NameComponent = typeof(PictureBlock).FullName,
            HeightSpawn = 1,
            
        };
        
        public static ExhibitDto InfoBox = new ExhibitDto()
        {
            Id = 2,
            Icon = null,
            Name = "InfoBox",
            IsBlock = false
        };
        
        public static ExhibitDto Thumb = new ExhibitDto()
        {
            Id = 3,
            Icon = null,
            Name = "Thumb",
            IsBlock = false
        };
        
        public static ExhibitDto Floor = new ExhibitDto()
        {
            Id = 4,
            Icon = null,
            Name = "Floor",
            IsBlock = true
        };
        
        public static ExhibitDto Celling = new ExhibitDto()
        {
            Id = 5,
            Icon = null,
            Name = "Celling",
            IsBlock = true
        };
        
        public static ExhibitDto Wall = new ExhibitDto()
        {
            Id = 6,
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