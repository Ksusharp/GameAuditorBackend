namespace Core.CommonModels.Enums
{
    public class TagNavigation : EntityBase
    {
        public Guid PostId { get; set; }

        public Post Post { get; set; }

        public Guid TagId { get; set; }

        public Tag TagName { get; set; }
    }
}
