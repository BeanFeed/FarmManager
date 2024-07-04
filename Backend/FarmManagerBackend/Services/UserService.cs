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

    #region AdminServices

    public async Task CreateUser(CreateUserModel userData)
    {
        User user = new User()
        {
            Name = userData.Name,
            Role = userData.Role,
            PassHash = BCrypt.Net.BCrypt.HashPassword(userData.Password)
        };

        await managerContext.Users.AddAsync(user);

        await managerContext.SaveChangesAsync();
    }

    public async Task DeleteUser(int userId)
    {
        #region Get User

        User? user = await managerContext.Users.FindAsync(userId);

        if (user is null) throw new UserException("Couldn't find user");
        
        #endregion

        managerContext.Users.Remove(user);

        await managerContext.SaveChangesAsync();
    }

    public async Task ChangeUserPassword(ChangeUserPasswordModel passwordData)
    {
        #region Get User

        User? user = await managerContext.Users.FindAsync(passwordData.UserId);

        if (user is null) throw new UserException("Couldn't find user");

        #endregion

        #region Change Password

        user.PassHash = BCrypt.Net.BCrypt.HashPassword(passwordData.NewPassword);

        managerContext.Users.Update(user);
        
        #endregion

        await managerContext.SaveChangesAsync();
    }

    #endregion
    

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