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
        var jwtService = context.HttpContext.RequestServices.GetService<JwtService>();
        
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
        
        if (token is null && rToken is null)
        {
            context.Result = new UnauthorizedObjectResult("User not logged in");
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }
        
        try
        {
            if (token is null) throw new UserException("Session Invalid");
            await jwtService!.DecodeToken(token);
            context.HttpContext.Items.Add("newToken", token);
        }
        catch (UserException e)
        {
            if (e.Message == "Session Invalid")
            {
                try
                {
                    var tokens = await userService!.Refresh(rToken);
                    context.HttpContext.Response.Cookies.Append("authToken", tokens[0], jwtOpt);
                    context.HttpContext.Response.Cookies.Append("rAuthToken", tokens[1], rJwtOpt);
                    context.HttpContext.Items.Add("newToken", tokens[0]);
                    token = tokens[0];
                }
                catch (UserException e2)
                {
                    context.Result = new BadRequestObjectResult(e2.Message);
                    context.HttpContext.Response.Cookies.Delete("authToken");
                    context.HttpContext.Response.Cookies.Delete("rAuthToken");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(e.Message);
                return;
            }
            
        }
        
        try
        {
            _user = await userService!.Me(token, true);
            
            context.HttpContext.Items.Add("User", _user);
        }
        catch (UserException e)
        {
            context.Result = new BadRequestObjectResult(e.Message);
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }

        
        
        await next();
    }
}
