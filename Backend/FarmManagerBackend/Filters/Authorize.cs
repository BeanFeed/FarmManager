using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Extensions;

namespace FarmManagerBackend.Filters;

public class Authorize : Attribute, IAsyncActionFilter
{
    private Roles _role = Roles.Member;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var auth = context.HttpContext.Request.Headers["Authorization"];
        
        if (auth.ToString().Length == 0)
        {
            context.Result = new UnauthorizedObjectResult("User not logged in");
            return;
        }
        
        var token = auth.ToString().Split(',')[0];
        var rToken = auth.ToString().Split(',')[1];
        var userService = context.HttpContext.RequestServices.GetService<UserService>();
        
        User user;

        try
        {
            user = await userService.Me(token);
        }
        catch (UserException e)
        {
            context.Result = new BadRequestObjectResult(e.Message);
            return;
        }

        if (!Enum.TryParse(user.Role, out Roles userRole))
        {
            throw new Exception($"Unknown role \"{user.Role}\" for User \"{user.Id}\"");
        }
        
        if (!Utils.RoleAuthorized(userRole, _role))
        {
            context.Result = new UnauthorizedObjectResult($"Requires {_role.GetDisplayName()} privileges");
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