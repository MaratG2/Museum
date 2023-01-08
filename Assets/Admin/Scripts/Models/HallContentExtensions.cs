using Admin.GenerationMap;
using Admin.Utility;
using GenerationMap;

namespace Admin.Models
{
    public static class HallContentExtensions
    {
        public static ExhibitDto ToExhibitDto(this HallContent contentInfo)
        {
            return ExhibitsConstants.GetModelById(contentInfo.type);
        }
    }
}