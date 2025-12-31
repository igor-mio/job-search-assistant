using JobSearchAssistant.Api.Auth;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace JobSearchAssistant.Api;

public class OwnershipTestFunction
{
    private readonly JwtValidator _jwt;
    private readonly IConfiguration _config;

    public OwnershipTestFunction(JwtValidator jwt, IConfiguration config)
    {
        _jwt = jwt;
        _config = config;
    }

    [Function("rbac_ownership_test")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rbac/ownership/test/{ownerId}")] HttpRequestData req,
        string ownerId)
    {
        var rolesClaim = _config["Auth0:RolesClaim"] ?? "https://job-search-assistant/roles";

        var (ctx, error) = await Authz.RequireAuthAsync(req, _jwt, rolesClaim);
        if (error is not null) return error;

        var forbidden = await Authz.RequireOwnerOrSuperAdminAsync(req, ctx!, ownerId);
        if (forbidden is not null) return forbidden;

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { ok = true, ownerId, caller = ctx!.Sub, roles = ctx.Roles });
        return res;
    }
}
