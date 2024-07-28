using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Filters;
using FarmManagerBackend.Models;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerBackend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly SettingsService _settings;
    public UserController(UserService userService, SettingsService settings)
    {
        _userService = userService;
        _settings = settings;
    }
    
    [HttpPost]
    [Authenticate]
    [Authorize(Role = Roles.Owner)]
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
    
    [HttpDelete]
    [Authenticate]
    [Authorize(Role = Roles.Owner)]
    public async Task<IActionResult> DeleteUser(int userId)
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
    
    [HttpGet]
    [Authenticate]
    [Authorize(Role = Roles.Owner)]
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
    
    [HttpGet]
    [Authenticate]
    [Authorize(Role = Roles.Owner)]
    public async Task<IActionResult> GetUsers(string? name)
    {
        return Ok(name == null ? await _userService.GetUsers() : await _userService.GetUsers(name));
    }
    
    [HttpPost]
    [Authenticate]
    [Authorize(Role = Roles.Owner)]
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
                Expires = DateTimeOffset.UtcNow.AddHours(2),
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

    [HttpPost]
    [Authenticate]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = HttpContext.Items["newToken"]!.ToString();
            await _userService.Logout(token);
            Response.Cookies.Delete("authToken");
            Response.Cookies.Delete("rAuthToken");
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("User logged out");
    }
    
    [HttpPost]
    [Authenticate]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel passwordData)
    {
        try
        {
            User user = (User)HttpContext.Items["User"]!;

            await _userService.ChangePassword(passwordData, user.Id);

            return Ok("Changed password");
        }
        catch (UserException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authenticate]
    public async Task<IActionResult> Me()
    {
        return Ok(await _userService.Me(HttpContext.Items["newToken"]!.ToString()!));
    }
    
    
}