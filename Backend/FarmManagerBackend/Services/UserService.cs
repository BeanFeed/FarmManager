using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerBackend.Services;

public class UserService
{
    private readonly ManagerContext _managerContext;
    private readonly JwtService _jwtService;
    
    public UserService(ManagerContext context, JwtService jwtService)
    {
        _managerContext = context;
        _jwtService = jwtService;
    }

    #region AdminServices

    public async Task CreateUser(CreateUserModel userData)
    {
        #region Check Existing User

        User? eUser = await _managerContext.Users.Where(x => x.Name == userData.Name).FirstAsync();

        if (eUser != null) throw new UserException("Name already used");

        #endregion
        
        User user = new User()
        {
            Name = userData.Name,
            Role = userData.Role,
            PassHash = BCrypt.Net.BCrypt.HashPassword(userData.Password)
        };

        await _managerContext.Users.AddAsync(user);

        await _managerContext.SaveChangesAsync();
    }

    public async Task DeleteUser(int userId)
    {
        #region Get User

        User? user = await _managerContext.Users.FindAsync(userId);

        if (user is null) throw new UserException("Couldn't find user");
        
        #endregion

        _managerContext.Users.Remove(user);

        await _managerContext.SaveChangesAsync();
    }

    public async Task ChangeUserPassword(ChangeUserPasswordModel passwordData)
    {
        #region Get User

        User? user = await _managerContext.Users.FindAsync(passwordData.UserId);

        if (user is null) throw new UserException("Couldn't find user");

        #endregion

        #region Change Password

        user.PassHash = BCrypt.Net.BCrypt.HashPassword(passwordData.NewPassword);

        _managerContext.Users.Update(user);
        
        #endregion

        await _managerContext.SaveChangesAsync();
    }

    #endregion

    public async Task<User> GetUser(int userId)
    {
        User? user = await _managerContext.Users.FindAsync(userId);
        if (user is null) throw new UserException("User not found");
        user.PassHash = null!;
        return user;
    }

    public async Task ModifyUser(ModifyUserModel userData)
    {
        #region Get User

        User? user = await _managerContext.Users.FindAsync(userData.Id);

        if (user is null) throw new UserException("User not found");

        #endregion

        #region Modify Data

        if (userData.Name is not null)
        {
            //Check if name already exists
            User? existingUser = await _managerContext.Users.Where(x => x.Name == userData.Name).FirstAsync();
            if (existingUser is not null) throw new UserException("Name already used");

            user.Name = userData.Name;
        }

        if (userData.Role is not null)
        {
            if (userData.Role == "owner") throw new UserException("Cannot make user an Owner");
            if (user.Role == "owner") throw new UserException("Cannot downgrade Owner");
            
            user.Role = userData.Role;
        }

        if (userData.Password is not null)
        {
            user.PassHash = BCrypt.Net.BCrypt.HashPassword(userData.Password);
        }
        #endregion

        _managerContext.Users.Update(user);

        await _managerContext.SaveChangesAsync();
    }

    public async Task<string[]> Login(UserLoginModel userData)
    {
        #region Get User

        User? user = await _managerContext.Users.Where(x => x.Name == userData.Name).FirstAsync();

        if (user is null) throw new UserException("User not found");

        #endregion

        #region Check Password

        if (!BCrypt.Net.BCrypt.Verify(userData.Password, user.PassHash)) throw new UserException("Invalid Password");

        #endregion

        var tokens = _jwtService.EncodeToken(user);

        return tokens;
    }

    public async Task<string[]> Refresh(string rToken)
    {
        if (!await _jwtService.IsRefreshValid(rToken)) throw new UserException("Session Invalid");

        User user;
        try
        {
            user = await _jwtService.DecodeRefreshToken(rToken);
        }
        catch (UserException e)
        {
            throw;
        }

        return _jwtService.EncodeToken(user);
    }

    public async Task ChangePassword(ChangePasswordModel passwordData, int id)
    {
        #region Check for user

        User? user = await _managerContext.Users.FindAsync(id);

        if (user is null) throw new UserException("User not found");

        #endregion

        #region Change Password

        if (BCrypt.Net.BCrypt.Verify(passwordData.OldPassword, user.PassHash))
            user.PassHash = BCrypt.Net.BCrypt.HashPassword(passwordData.NewPassword);

        #endregion

        await _managerContext.SaveChangesAsync();
    }

    public async Task<string> GetUserRole(int userId)
    {
        #region Check for user

        User? user = await _managerContext.Users.FindAsync(userId);

        if (user is null) throw new UserException("User not found");

        #endregion

        return user.Role;
    }

    public async Task<User> Me(string jwt)
    {
        #region Check for user

        try
        {
            User? user = await _jwtService.DecodeToken(jwt);
            return user;
        }
        catch (UserException e)
        {
            throw;
        }
        #endregion
    }
}