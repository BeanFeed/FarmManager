using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FarmManagerBackend.Filters;

public class Authenticate : Attribute, IAsyncActionFilter
{
    private User _user;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var settings = context.HttpContext.RequestServices.GetService<SettingsService>();
        var userService = context.HttpContext.RequestServices.GetService<UserService>();
        
        var jwtOpt = new CookieOptions()
            {
                Domain = context.HttpContext.Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15),
                Secure = settings.JwtConfig.Secure,
                SameSite = Utils.GetSSM(settings.JwtConfig.SSM)
                
            };
            
            var rJwtOpt = new CookieOptions()
            {
                Domain = context.HttpContext.Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = settings.JwtConfig.Secure,
                SameSite = Utils.GetSSM(settings.JwtConfig.SSM)
                
            };
            
        var token = context.HttpContext.Request.Cookies["authToken"];
        var rToken = context.HttpContext.Request.Cookies["rAuthToken"];
        
        if (token is null || rToken is null)
        {
            context.Result = new UnauthorizedObjectResult("User not logged in");
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }
        
        try
        {
            _user = await userService.Me(token);
        }
        catch (UserException e)
        {
            context.Result = new BadRequestObjectResult(e.Message);
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }

        try
        {
            var tokens = await userService.Refresh(rToken);
            context.HttpContext.Response.Cookies.Append("authToken", tokens[0]);
            context.HttpContext.Response.Cookies.Append("rAuthToken", tokens[1]);
        }
        catch (UserException e)
        {
            context.Result = new BadRequestObjectResult(e.Message);
            return;
        }
        
        await next();
    }

    public virtual User User => _user;
}
