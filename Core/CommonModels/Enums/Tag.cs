using System.ComponentModel.DataAnnotations;

namespace Core.CommonModels.Enums
{
    public class Tag : EntityBase
    {
        [Required]
        public string TagName { get; set; }

        public IEnumerable<TagNavigation> TagNavigation { get; set; }
    }
}
