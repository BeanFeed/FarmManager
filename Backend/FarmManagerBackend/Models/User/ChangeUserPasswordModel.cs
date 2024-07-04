namespace FarmManagerBackend.Models.User;

//This model is for the service used by admins
public class ChangeUserPasswordModel
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = null!;
}