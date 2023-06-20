using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Posts.Api.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        [Required]
        [MaxLength(12000)]
        public string Content { get; set; }

        [Required]
        public IEnumerable<TagViewModel> Tags { get; set; }

        [NotNull]
        public DateTime? CreatedDate { get; set; }

        [NotNull]
        public DateTime? UpdatedDate { get; set; }
    }
}
