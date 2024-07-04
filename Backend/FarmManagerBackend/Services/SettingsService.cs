using FarmManagerBackend.Models.Settings;
using Microsoft.Extensions.Options;

namespace FarmManagerBackend.Services;

public class SettingsService
{
    public readonly JwtConfig JwtConfig;
    
    public SettingsService(IOptions<JwtConfig> jwtConfig)
    {
        JwtConfig = jwtConfig.Value;
    }
}