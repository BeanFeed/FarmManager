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
        
        var auth = context.HttpContext.Request.Headers["Authorization"];

        context.HttpContext.Response.Headers["Authorization"] = context.HttpContext.Request.Headers["Authorization"];
        
        if (auth.ToString().Length == 0)
        {
            context.Result = new UnauthorizedObjectResult("User not logged in");
            return;
        }
        
        var token = auth.ToString().Split(',')[0];
        var rToken = auth.ToString().Split(',')[1];
        
        
        
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
                    context.HttpContext.Response.Headers.Authorization = $"{tokens[0]},{tokens[1]}";
                    context.HttpContext.Items.Add("newToken", tokens[0]);
                    token = tokens[0];
                }
                catch (UserException e2)
                {
                    context.Result = new BadRequestObjectResult(e2.Message);
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
            return;
        }

        
        
        await next();
    }
}
