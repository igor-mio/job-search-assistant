namespace JobSearchAssistant.Api.Auth;

public sealed record AuthContext(string Sub, string [] Roles)
{
    public bool IsSuperAdmin => Roles.Contains("SUPER_ADMIN");
}