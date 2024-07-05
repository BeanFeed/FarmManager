namespace FarmManagerBackend.Models.User;

public class ModifyUserModel
{
    public int Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Role { get; set; } = null!;
    public string? Password { get; set; } = null!;
}