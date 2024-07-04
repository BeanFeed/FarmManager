namespace FarmManagerBackend.Models.Settings;

public class JwtConfig
{
    public string Key { get; set; } = null!;
    public string RKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public bool Secure { get; set; }
    public string SSM { get; set; } = null!;
}