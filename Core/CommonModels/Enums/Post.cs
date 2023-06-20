using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.CommonModels.Enums
{
    public class Post : EntityBase
    {
        [Required]
        public string OwnerId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        [Required]
        [MaxLength(12000)]
        public string Content { get; set; }

        [Required]
        public IEnumerable<TagNavigation> TagNavigation { get; set; }

        [NotNull]
        public DateTime? CreatedDate { get; set; }

        [NotNull]
        public DateTime? UpdatedDate { get; set; }
    }
}
