using System.ComponentModel.DataAnnotations;

namespace Posts.App.Model
{
    public class TagRequestViewModel
    {
        public Guid Id { get; set; }
        public string TagName { get; set; }
    }
}
