using AutoMapper;
using Core.CommonModels.Enums;
using Core.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.ViewModels;
using Posts.App.Model;
using Posts.App.Queries;

namespace Posts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IEntityRepository<Post> _postRepository;
        private readonly IEntityRepository<Tag> _tagRepository;
        private readonly IEntityRepository<TagNavigation> _tagNavigationRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly PostsQueries _postsQueries;

        public PostsController(IEntityRepository<Post> postRepository, IEntityRepository<Tag> tagRepository,
            IEntityRepository<TagNavigation> tagNavigationRepository, IMapper mapper, IUserService userService, PostsQueries postsQueries)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _tagNavigationRepository = tagNavigationRepository;
            _mapper = mapper;
            _userService = userService;
            _postsQueries = postsQueries;
        }

        [AllowAnonymous]
        [HttpGet("allPosts")]
        public IEnumerable<PostRequestViewModel> GetAllPosts()
        {
            return _postsQueries.GetAllPosts();
        }

        [AllowAnonymous]
        [HttpGet("postWithId")]
        public Post Get(Guid id)
        {
            return _postRepository.Get(id);
        }

        [Authorize]
        [HttpPost("createPost")]
        public IActionResult Create(CreatePostViewModel postEntity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var newPost = _mapper.Map<Post>(postEntity);
                newPost.OwnerId = _userService.GetMyId();
                _postRepository.Create(newPost);
                _postRepository.Save();
                if (postEntity.Tags.Any())
                {
                    var existTags = postEntity.Tags.Select(x => x.Tag).ToList();
                    var tags = _tagRepository.GetAll().Where(x => existTags.Contains(x.TagName)).ToList();
                    if (tags.Any())
                    {
                        var newTagsNav = tags.Select(x => new TagNavigation() { TagId = x.Id, PostId = newPost.Id });
                        _tagNavigationRepository.CreateRange(newTagsNav);
                    }
                }
                _tagNavigationRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("updatePost")]
        public IActionResult Update(UpdatePostViewModel postEntity, Guid id)
        {
            var post = _postRepository.Get(id);
            if (post.OwnerId == _userService.GetMyId())
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updatePost = _mapper.Map<UpdatePostViewModel, Post>(postEntity, post);

                _postRepository.Update(updatePost);
                _postRepository.Save();

                if (postEntity.Tags.Any())
                {
                    var postTags = _tagNavigationRepository.GetAll().Where(x => x.PostId == id);

                    foreach (var i in postTags)
                    {
                        _tagNavigationRepository.Delete(i.Id);
                    }

                    var existTags = postEntity.Tags.Select(x => x.Tag).ToList();
                    var tags = _tagRepository.GetAll().Where(x => existTags.Contains(x.TagName)).ToList();

                    if (tags.Any())
                    {
                        var newTagsNav = tags.Select(x => new TagNavigation() { TagId = x.Id, PostId = updatePost.Id });
                        _tagNavigationRepository.CreateRange(newTagsNav);
                    }

                    else return BadRequest();
                }
                else return BadRequest();

                _tagNavigationRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }
            else
                return BadRequest("You are not the owner of the post");
        }

        [Authorize]
        [HttpDelete("deletePostWithId")]
        public IActionResult Delete(Guid id)
        {
            var post = _postRepository.Get(id);
            var postTags = _tagNavigationRepository.GetAll().Where(x => x.PostId == id);

            if (post.OwnerId == _userService.GetMyId())
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    if (postTags.Any())
                    {
                        foreach (var postTag in postTags)
                        {
                            _tagNavigationRepository.Delete(postTag.Id);
                        }
                        _tagNavigationRepository.Save();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                _postRepository.Delete(id);
                _postRepository.Save();
                return Ok();
            }
            else
                return BadRequest("You are not the owner of the post");
        }

        [AllowAnonymous]
        [HttpGet("getTagsFromPost")]
        public IEnumerable<TagAndTagIdViewModel> GetTags(Guid id)
        {
            var postTags = _tagNavigationRepository.GetAll().Where(x => x.PostId == id);

            if (postTags.Any())
            {

                var tagAndTagId =
                    from pt in postTags
                    join t in _tagRepository.GetAll() on pt.TagId equals t.Id
                    select new TagAndTagIdViewModel
                    {
                        Tag = t.TagName,
                        TagId = pt.Id
                    };

                return tagAndTagId;
            }
            else return Enumerable.Empty<TagAndTagIdViewModel>();
        }

        [AllowAnonymous]
        [HttpPost("getPostWithTagOrSearch")]
        public IEnumerable<PostRequestViewModel> GetSearch([FromBody] SearchPostViewModel searchPost)
        {
            var posts = _postRepository.GetAll();

            if (!string.IsNullOrEmpty(searchPost.SearchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchPost.SearchString)).ToList();
            }

            if (searchPost.Tags != null && searchPost.Tags.Any())
            {
                var tags = searchPost.Tags.Select(x => x.Tag);
                var tagsId = _tagRepository.GetAll().Where(tag => tags.Contains(tag.TagName)).Select(tag => tag.Id);

                var postsId = _tagNavigationRepository.GetAll().Where(tagNav => tagsId.Contains(tagNav.TagId)).Select(tagNav => tagNav.PostId);

                posts = posts.Where(posts => postsId.Contains(posts.Id));

            }

            return _postsQueries.GetSearch(posts);

        }
    }
}
