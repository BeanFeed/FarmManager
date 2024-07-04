using DAL.Entities;
using FarmManagerBackend.Models.User;

namespace FarmManagerBackend.Services.Interfaces;

public interface IUserService
{
    #region Admin Services

    public Task CreateUser(CreateUserModel userData); //Admin creates new users
    public Task DeleteUser(int userId); //Delete unneeded users
    public Task ChangeUserPassword(ChangeUserPasswordModel passwordData); //Change password for other users

    #endregion
    
    public Task<string[]> Login(UserLoginModel userData); //All users login
    public Task<string[]> Refresh(string rToken); //Refresh Access and Refresh tokens
    public Task ChangePassword(ChangePasswordModel passwordData);
    public Task<string> GetUserRole(int userId);
    public Task<User> Me(string jwt); //Get basic user info
}