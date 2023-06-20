namespace Posts.Api.ViewModels
{
    public class SearchPostViewModel
    {
        public string? SearchString { get; set; }

        public IEnumerable<TagViewModel>? Tags { get; set; }

    }
}
