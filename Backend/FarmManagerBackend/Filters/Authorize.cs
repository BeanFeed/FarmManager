using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FarmManagerBackend.Filters;

public class Authorize : Attribute, IAsyncActionFilter
{
    private Roles _role = Roles.Member;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Request.Cookies["authToken"];
        var rToken = context.HttpContext.Request.Cookies["rAuthToken"];
        var userService = context.HttpContext.RequestServices.GetService<UserService>();

        if (token is null || rToken is null)
        {
            context.Result = new UnauthorizedObjectResult("User not logged in");
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }
        
        User user;

        try
        {
            user = await userService.Me(token);
        }
        catch (UserException e)
        {
            context.Result = new BadRequestObjectResult(e.Message);
            context.HttpContext.Response.Cookies.Delete("authToken");
            context.HttpContext.Response.Cookies.Delete("rAuthToken");
            return;
        }

        if (!Enum.TryParse(user.Role, out Roles userRole))
        {
            throw new Exception($"Unknown role \"{user.Role}\" for User \"{user.Id}\"");
        }
        
        if (!Utils.RoleAuthorized(userRole, _role))
        {
            context.Result = new UnauthorizedObjectResult($"Requires {nameof(_role)} privileges");
            return;
        }

        await next();
    }
    
    public virtual Roles Role
    {
        get => _role;
        set => _role = value;
    }
}