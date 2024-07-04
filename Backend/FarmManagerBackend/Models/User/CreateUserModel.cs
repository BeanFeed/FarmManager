namespace FarmManagerBackend.Models.User;

public class CreateUserModel
{
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Password { get; set; } = null!;
}