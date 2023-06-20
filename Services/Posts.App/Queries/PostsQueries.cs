using Core.CommonModels.Enums;
using Core.Db;
using Posts.App.Model;

namespace Posts.App.Queries
{
    public class PostsQueries
    {

        private readonly IEntityRepository<Post> _postRepository;
        private readonly IEntityRepository<Tag> _tagRepository;
        private readonly IEntityRepository<TagNavigation> _tagNavigationRepository;


        public PostsQueries(IEntityRepository<Post> postRepository, IEntityRepository<Tag> tagRepository,
            IEntityRepository<TagNavigation> tagNavigationRepository)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _tagNavigationRepository = tagNavigationRepository;

        }

        public IEnumerable<PostRequestViewModel> GetAllPosts()
        {

            var result = (
                            from posts in _postRepository.GetAll()
                            join tagNav in _tagNavigationRepository.GetAll() on posts.Id equals tagNav.PostId into gj

                            from postWithTagId in gj
                            join tags in _tagRepository.GetAll() on postWithTagId.TagId equals tags.Id

                            select new PostRequestViewModel()
                            {
                                OwnerId = posts.OwnerId,

                                Title = posts.Title,

                                Content = posts.Content,

                                PostId = posts.Id,

                                TagsId = gj.Select(tag => tag.TagId),

                                Tags = gj.Select(tag => tag.TagName.TagName),

                                CreatedDate = posts.CreatedDate,

                                UpdatedDate = posts.UpdatedDate,

                            }).GroupBy(p => p.PostId).Select(gr => gr.First()).ToList();
            return result;
        }

        public IEnumerable<TagRequestViewModel> GetAllTags()
        {

            var result = (
                            from tags in _tagRepository.GetAll()

                            select new TagRequestViewModel()
                            {
                                Id = tags.Id,

                                TagName = tags.TagName,

                            }).GroupBy(p => p.Id).Select(gr => gr.First()).ToList();
            return result;
        }

        public IEnumerable<PostRequestViewModel> GetSearch(IEnumerable<Post> posts)
        {
            var result = (
              from post in posts
              join tagNav in _tagNavigationRepository.GetAll() on post.Id equals tagNav.PostId into gj

              from postWithTagId in gj
              join tags in _tagRepository.GetAll() on postWithTagId.TagId equals tags.Id

              select new PostRequestViewModel()
              {
                  OwnerId = post.OwnerId,

                  Title = post.Title,

                  Content = post.Content,

                  PostId = post.Id,

                  TagsId = gj.Select(tag => tag.TagId),

                  Tags = gj.Select(tag => tag.TagName.TagName),

                  CreatedDate = post.CreatedDate,

                  UpdatedDate = post.UpdatedDate,

              }).GroupBy(p => p.PostId).Select(gr => gr.First()).ToList();

            return result;

        }
    }
}
