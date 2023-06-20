namespace Posts.App.Model
{
    public class PostRequestViewModel
    {
        public Guid? PostId { get; set; }

        public string OwnerId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<Guid> TagsId { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
