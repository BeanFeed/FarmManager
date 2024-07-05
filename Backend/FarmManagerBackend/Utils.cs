using FarmManagerBackend.Models.User;

namespace FarmManagerBackend;

public class Utils
{
    public static SameSiteMode GetSSM(string ssm)
    {
        if (ssm == null || ssm.ToLower() == "none") return SameSiteMode.None;
        else return SameSiteMode.Lax;
    }

    public static bool RoleAuthorized(Roles role, Roles minimumRole)
    {
        return role >= minimumRole;
    }
}