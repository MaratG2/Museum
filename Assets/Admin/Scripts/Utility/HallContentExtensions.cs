using System.Drawing;
using Admin.GenerationMap;
using Admin.Models;
using GenerationMap;

namespace Admin.Utility
{
    public static class HallContentExtensions
    {
        public static ExhibitDto ToExhibitDto(this HallContent hallContent)
        {
            var constExhibit = ExhibitsConstants.GetModelById(hallContent.type);
            return new ExhibitDto()
            {
                Id = constExhibit.Id,
                Name = constExhibit.Name,
                NameComponent = constExhibit.NameComponent,
                HeightSpawn = constExhibit.HeightSpawn,
                IsBlock = constExhibit.IsBlock,
                LocalPosition = new Point(hallContent.pos_x, hallContent.pos_z),
                LinkOnImage = hallContent.image_url,
                TextContentFirst = hallContent.title,
                Description = hallContent.image_desc,
            };
        }
    }
}