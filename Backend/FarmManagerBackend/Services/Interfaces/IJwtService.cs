using DAL.Entities;

namespace FarmManagerBackend.Services.Interfaces;

public interface IJwtService
{
    public string[] EncodeToken(User user);
    public Task<User> DecodeToken(string eToken);
    public Task<User> DecodeRefreshToken(string rToken);
    public Task<bool> IsRefreshValid(string rToken);
}