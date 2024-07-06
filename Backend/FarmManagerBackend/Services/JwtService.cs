using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.Settings;
using FarmManagerBackend.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FarmManagerBackend.Services;

public class JwtService
{
    private readonly JwtConfig _jwtSettings;
    private readonly ManagerContext managerContext;
    
    public JwtService(IOptions<JwtConfig> jwtSettings, ManagerContext context)
    {
        _jwtSettings = jwtSettings.Value;
        managerContext = context;
    }

    private async Task InvalidateOldSession(Guid sessionId)
    {
        Session? session = await managerContext.Sessions.FindAsync(sessionId);
        if (session is null) throw new UserException("Session not found");

        session.Valid = false;
        
        managerContext.Sessions.Update(session);
        await managerContext.SaveChangesAsync();
    }
    
    public async Task<string[]> EncodeToken(User user)
    {
        Console.WriteLine(_jwtSettings.Key);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var rsecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var rCredentials = new SigningCredentials(rsecurityKey, SecurityAlgorithms.HmacSha256);
        
        var sGuid = Guid.NewGuid();
        var sessionGuid = sGuid.ToString();

        Session session = new Session()
        {
            Valid = true,
            UserId = user.Id,
            SessionId = sGuid
        };
        
        await managerContext.Sessions.AddAsync(session);
        await managerContext.SaveChangesAsync();
        var claims = new[]
        {
            new Claim("Name", user.Name),
            new Claim("Role", user.Role),
            new Claim("Id", user.Id.ToString()),
            new Claim("SessionId", sessionGuid)
            
        };
        
        var rClaims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("SessionId", sessionGuid)
            
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
            );

        var rToken = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            rClaims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: rCredentials
            );
        string[] tokens = { new JwtSecurityTokenHandler().WriteToken(token), new JwtSecurityTokenHandler().WriteToken(rToken) };
        return tokens;
    }
    
    
    public async Task<User> DecodeToken(string eToken, bool keepHash = false)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var handler = new JwtSecurityTokenHandler();
                   
        TokenValidationResult result = await handler.ValidateTokenAsync(eToken,new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        });
        if (!result.IsValid) throw new UserException("Session Invalid");
        JwtSecurityToken token = handler.ReadJwtToken(eToken);
        
        var sessionId = Guid.Parse(token.Claims.Where(claim => claim.Type == "SessionId").ToArray()[0].Value);
        var name = token.Claims.Where(claim => claim.Type == "Name").ToArray()[0].Value;
        var id = int.Parse(token.Claims.Where(claim => claim.Type == "Id").ToArray()[0].Value);
        var usr = await SearchUser(id);
        if (usr == null) throw new UserException("User not found");
        User cUsr = new User()
        {
            Id = usr.Id,
            Name = usr.Name,
            Role = usr.Role,
            PassHash = ""
        };
        return cUsr;
        
        
    }

    public async Task<User> DecodeRefreshToken(string rToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RKey));
        var handler = new JwtSecurityTokenHandler();
                   
        TokenValidationResult result = await handler.ValidateTokenAsync(rToken,new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        });
        if (!result.IsValid) throw new UserException("Session Invalid");
        JwtSecurityToken token = handler.ReadJwtToken(rToken);
        
        var sessionId = Guid.Parse(token.Claims.Where(claim => claim.Type == "SessionId").ToArray()[0].Value);
        var userId = int.Parse(token.Claims.Where(claim => claim.Type == "UserId").ToArray()[0].Value);

        var user = await SearchUser(userId);

        if (user is null) throw new UserException("User not found");

        User cUsr = new User()
        {
            Id = user.Id,
            Name = user.Name,
            Role = user.Role,
            PassHash = ""
        };

        await InvalidateOldSession(sessionId);
        
        return cUsr;
    }

    public async Task<bool> IsRefreshValid(string rToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RKey));
        var handler = new JwtSecurityTokenHandler();
                   
        TokenValidationResult result = await handler.ValidateTokenAsync(rToken,new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        });
        if (!result.IsValid) return false;
        JwtSecurityToken token = handler.ReadJwtToken(rToken);
        
        var sessionId = Guid.Parse(token.Claims.Where(claim => claim.Type == "SessionId").ToArray()[0].Value);
        var userId = int.Parse(token.Claims.Where(claim => claim.Type == "UserId").ToArray()[0].Value);

        return await IsValidSession(sessionId, userId);
    }

    private async Task<bool> IsValidSession(Guid sessionId, int userId)
    {
        Session? session = await managerContext.Sessions.FindAsync(sessionId);

        if (session is null) return false;

        if (session.UserId != userId || !session.Valid) return false;

        return true;
    }

    private async Task<User?> SearchUser(int id)
    {
        User? user = await managerContext.Users.FindAsync(id);

        if (user is null) return null;

        return user;
    }
}