namespace FarmManagerBackend;

public class Utils
{

    private static readonly Dictionary<string, int> Roles = new Dictionary<string, int>()
    {
        {"member",0},
        {"admin",1},
        {"owner",2}
    };
    
    public static SameSiteMode GetSSM(string ssm)
    {
        if (ssm == null || ssm.ToLower() == "none") return SameSiteMode.None;
        else return SameSiteMode.Lax;
    }

    public static bool RoleAuthorized(string role, string minimumRole)
    {
        return Roles[role] >= Roles[minimumRole];
    }
}