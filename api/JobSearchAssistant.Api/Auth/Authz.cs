using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace JobSearchAssistant.Api.Auth;

public static class Authz
{
    /// <summary>
    /// Validates JWT and returns AuthContext (sub + roles). Returns 401 response if missing/invalid.
    /// </summary>
    public static async Task<(AuthContext? ctx, HttpResponseData? error)> RequireAuthAsync(
        HttpRequestData req,
        JwtValidator jwt,
        string rolesClaimType)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            return (null, await UnauthorizedAsync(req));

        var auth = authHeaders.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return (null, await UnauthorizedAsync(req));

        var token = auth["Bearer ".Length..].Trim();

        ClaimsPrincipal principal;
        try
        {
            principal = await jwt.ValidateAsync(token, req.FunctionContext.CancellationToken);
        }
        catch
        {
            return (null, await UnauthorizedAsync(req));
        }

        // Prefer .NET canonical claim for user id
        var sub =
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(sub))
            return (null, await UnauthorizedAsync(req));

        var roles = principal.Claims
            .Where(c => c.Type == rolesClaimType)
            .SelectMany(c =>
            {
                // roles can be: "SUPER_ADMIN" or JSON array ["SUPER_ADMIN"]
                if (c.Value.StartsWith("["))
                {
                    try { return JsonSerializer.Deserialize<string[]>(c.Value) ?? Array.Empty<string>(); }
                    catch { return Array.Empty<string>(); }
                }
                return new[] { c.Value };
            })
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return (new AuthContext(sub, roles), null);
    }

    /// <summary>
    /// SUPER_ADMIN only. Returns 403 if authenticated but not authorized.
    /// </summary>
    public static async Task<HttpResponseData?> RequireSuperAdminAsync(HttpRequestData req, AuthContext ctx)
    {
        if (ctx.IsSuperAdmin) return null;
        return await ForbiddenAsync(req, "SUPER_ADMIN role required");
    }

    /// <summary>
    /// Owner (ownerId == ctx.Sub) OR SUPER_ADMIN. Returns 403 if not allowed.
    /// </summary>
    public static async Task<HttpResponseData?> RequireOwnerOrSuperAdminAsync(HttpRequestData req, AuthContext ctx, string ownerId)
    {
        if (ctx.IsSuperAdmin) return null;
        if (string.Equals(ctx.Sub, ownerId, StringComparison.Ordinal)) return null;

        return await ForbiddenAsync(req, "Owner or SUPER_ADMIN required");
    }

    private static async Task<HttpResponseData> UnauthorizedAsync(HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.Unauthorized);
        await res.WriteAsJsonAsync(new { error = "Unauthorized" });
        return res;
    }

    private static async Task<HttpResponseData> ForbiddenAsync(HttpRequestData req, string message)
    {
        var res = req.CreateResponse(HttpStatusCode.Forbidden);
        await res.WriteAsJsonAsync(new { error = "Forbidden", message });
        return res;
    }
}
