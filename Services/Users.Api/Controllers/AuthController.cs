using Core.CommonModels.Enums;
using Core.Db;
using GameAuditor.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Users.App.Models;

namespace Users.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration configuration, IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet("getMe")]
        public ActionResult<string> GetMe()
        {
            var UserName = _userService.GetMyName();
            return Ok(UserName);
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<ActionResult<User>> Register(RegisterViewModel request)
        {
            var existUser = await _userManager.FindByNameAsync(request.UserName);
            if (existUser != null)
                return BadRequest("User already exists.");

            var result = await _userManager.CreateAsync(new User()
            {
                Email = request.Email,
                UserName = request.UserName
            }, request.Password);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginViewModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (!result.Succeeded)
                return Unauthorized("Wrong password");

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Email, request.Email)
            };

            var refreshToken = CreateRefreshToken(claims);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse()
            {
                Token = GenerateAccessToken(claims),
                RefreshToken = refreshToken
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<string>> RefreshToken(string refreshToken)
        {
            var userName = _userService.GetMyName();
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.RefreshToken != refreshToken || user == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now) 
            {
                return Unauthorized("Token expired");
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Email, user.Email)
            };

            var newRefreshToken = CreateRefreshToken(claims);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse()
            {
                Token = GenerateAccessToken(claims),
                RefreshToken = newRefreshToken
            };

            return Ok(response);
        }

        private Token GenerateAccessToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(64));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var accessToken = new Token
            {
                AccessToken = jwt,
                AccessTokenExpiryTime = DateTime.Now.AddDays(1),
            }; 

            return accessToken;
            
        }

        private string CreateRefreshToken(List<Claim> claims)
        {

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
