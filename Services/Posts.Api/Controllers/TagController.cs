using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Db;
using Posts.Api.ViewModels;
using Core.CommonModels.Enums;
using Core.Db.Ef;
using Posts.App.Model;
using Posts.App.Queries;

namespace Posts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        public IEntityRepository<Tag> _tagRepository;
        public IEntityRepository<TagNavigation> _tagNavigationRepository;
        private readonly PostsQueries _postsQueries;

        public TagController(IEntityRepository<Tag> tagRepository, IEntityRepository<TagNavigation> tagNavigationRepository, PostsQueries postsQueries)
        {
            _tagRepository = tagRepository;
            _tagNavigationRepository = tagNavigationRepository;
            _postsQueries = postsQueries;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(TagViewModel newTag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                _tagRepository.Create(new Tag() { TagName = newTag.Tag });
                _tagRepository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("getAllTags")]
        public IEnumerable<TagRequestViewModel> GetAllTags()
        {
            return _postsQueries.GetAllTags();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteTagWithId")]
        public IActionResult Delete(Guid id)
        {
            var tag = _tagRepository.Get(id);
            var postTags = _tagNavigationRepository.GetAll().Where(x => x.TagName == tag);

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

            _tagRepository.Delete(id);
            _tagRepository.Save();

            return Ok();
        }
    }
}
