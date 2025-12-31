using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobSearchAssistant.Api.Auth;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Net;
using System.Security.Claims;

namespace JobSearchAssistant.Api;

public class MeFunction
{
    private readonly JwtValidator _jwt;
    private readonly IConfiguration _config;

    public MeFunction(JwtValidator jwt, IConfiguration config)
    {
        _jwt = jwt;
        _config = config;
    }

    [Function("Me")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")] HttpRequestData req)
    {
        // Authorization : Bearer <token>
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            return await Unauthorized(req);

        var auth = authHeaders.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return await Unauthorized(req);

        var token = auth["Bearer ".Length..].Trim();

        try
        {
            var principal = await _jwt.ValidateAsync(token, req.FunctionContext.CancellationToken);


            var sub =
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

            var rolesClaim = _config["Auth0:RolesClaim"] ?? "https://job-search-assistant/roles";

            //Auht0 Aciton usually sets roles as a JSON array in one class
            var roles = principal.Claims
                .Where(c => c.Type == rolesClaim)
                .SelectMany(c =>
                {
                    if (c.Value.StartsWith("["))
                    {
                        try { return JsonSerializer.Deserialize<string[]>(c.Value) ?? Array.Empty<string>(); }
                        catch { return Array.Empty<string>(); }
                    }
                    return new[] { c.Value };
                })
                .Distinct()
                .ToArray();

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { sub, roles });
            return res;
        }
        catch
        {
            return await Unauthorized(req);
        }
    }

    private static async Task<HttpResponseData> Unauthorized(HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.Unauthorized);
        await res.WriteAsJsonAsync(new { error = "Unauthorized" });
        return res;
    }
}
