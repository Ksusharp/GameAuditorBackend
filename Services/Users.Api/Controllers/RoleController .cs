using GameAuditor.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Db;
using Core.CommonModels.Enums;

namespace Users.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        private readonly IUserService _userService;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IUserService userService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("getRoles")]
        public async Task<IEnumerable<string>> Index()
        {
            var userName = _userService.GetMyName();
            var user = await _userManager.FindByNameAsync(userName);
            return await _userManager.GetRolesAsync(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("createRole")]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest("Null name");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteRole")]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                return Ok(result);
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult UserList() => Ok(_userManager.Users.ToList());

        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public async Task<IActionResult> Edit(Guid userId)
        {
            User user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id.ToString(),
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };

                return Ok(model);
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                if (await _roleManager.FindByNameAsync(roleName) != null)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    return Ok();
                }
                else return BadRequest("Role not found");
            }
            else return BadRequest("User not found");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("deleteRoleFromUser")]
        public async Task<IActionResult> DeleteToRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                    return Ok();
                }
                else return BadRequest("User does not have this role");
            }
            else return BadRequest("User not found");
        }
    }
}