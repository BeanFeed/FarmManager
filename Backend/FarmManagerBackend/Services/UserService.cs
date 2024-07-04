using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services.Interfaces;

namespace FarmManagerBackend.Services;

public class UserService : IUserService
{
    private readonly ManagerContext managerContext;
    
    public UserService(ManagerContext context)
    {
        managerContext = context;
    }
    
    public Task CreateUser(CreateUserModel userData)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(int userId)
    {
        throw new NotImplementedException();
    }

    public Task ChangeUserPassword(ChangeUserPasswordModel passwordData)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> Login(UserLoginModel userData)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> Refresh(string rToken)
    {
        throw new NotImplementedException();
    }

    public Task ChangePassword(ChangePasswordModel passwordData)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetUserRole(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<User> Me(string jwt)
    {
        throw new NotImplementedException();
    }
}