using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Filters;
using FarmManagerBackend.Models;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly SettingsService _settings;
    public UserController(UserService userService, SettingsService settings)
    {
        _userService = userService;
        _settings = settings;
    }
    
    [Authenticate]
    [Authorize(Role = "owner")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserModel userData)
    {
        try
        {
            await _userService.CreateUser(userData);
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("User Created");
    }

    [Authenticate]
    [Authorize(Role = "owner")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody]int userId)
    {
        try
        {
            await _userService.DeleteUser(userId);
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("User Deleted");
    }

    [Authenticate]
    [Authorize(Role = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetUser(int userId)
    {
        try
        {
            var user = await _userService.GetUser(userId);
            return Ok(user);
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authenticate]
    [Authorize(Role = "owner")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userService.GetUsers());
    }

    [Authenticate]
    [Authorize(Role = "owner")]
    [HttpPost]
    public async Task<IActionResult> ModifyUser(ModifyUserModel userData)
    {
        try
        {
            await _userService.ModifyUser(userData);
            return Ok("Updated user");
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel userData) 
    {
        try
        {
            var tokens = await _userService.Login(userData);

            #region Jwt Options

            var jwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15),
                Secure = _settings.JwtConfig.Secure,
                SameSite = Utils.GetSSM(_settings.JwtConfig.SSM)
            };
            
            var rJwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = _settings.JwtConfig.Secure,
                SameSite = Utils.GetSSM(_settings.JwtConfig.SSM)
            };

            #endregion
            
            Response.Cookies.Append("authToken", tokens[0], jwtOpt);
            Response.Cookies.Append("rAuthToken", tokens[1], rJwtOpt);
            
            return Ok("User logged in");
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authenticate]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel passwordData)
    {
        try
        {
            var user = await _userService.Me(Request.Cookies["authToken"]!);

            await _userService.ChangePassword(passwordData, user.Id);

            return Ok("Changed password");
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}