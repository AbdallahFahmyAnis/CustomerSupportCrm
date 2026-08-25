using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Crm.BuildingBlocks.Diagnostics;
using Crm.Contracts.Health;
using Crm.Contracts.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("downstream", client => client.Timeout = TimeSpan.FromSeconds(3));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "crm.bff";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect("/login");
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(context =>
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                context.ProxyRequest.Headers.Remove("X-Crm-User-Id");
                context.ProxyRequest.Headers.Remove("X-Crm-User-Email");
                context.ProxyRequest.Headers.Remove("X-Crm-User-Role");
                context.ProxyRequest.Headers.TryAddWithoutValidation(
                    "X-Crm-User-Id",
                    user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
                context.ProxyRequest.Headers.TryAddWithoutValidation(
                    "X-Crm-User-Email",
                    user.FindFirstValue(ClaimTypes.Email) ?? user.Identity.Name ?? string.Empty);
                context.ProxyRequest.Headers.TryAddWithoutValidation(
                    "X-Crm-User-Role",
                    user.FindFirstValue(ClaimTypes.Role) ?? string.Empty);

                var access = context.HttpContext.Request.Cookies["crm.at"];
                if (!string.IsNullOrWhiteSpace(access))
                {
                    context.ProxyRequest.Headers.Remove("Authorization");
                    context.ProxyRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {access}");
                }
            }

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();
app.UseCorrelationId();
app.UseAuthentication();
app.UseAuthorization();

const string RefreshCookie = "crm.rt";
const string AccessCookie = "crm.at";
var jsonOpts = new JsonSerializerOptions(JsonSerializerDefaults.Web);

static CookieOptions TokenCookieOptions(DateTimeOffset expires) => new()
{
    HttpOnly = true,
    Secure = false,
    SameSite = SameSiteMode.Lax,
    Expires = expires.UtcDateTime,
    IsEssential = true,
    Path = "/"
};

app.MapGet("/health", async (IHttpClientFactory httpFactory, IConfiguration config) =>
{
    var client = httpFactory.CreateClient("downstream");
    var probes = new (string Name, string Url)[]
    {
        ("identity", config["Services:Identity"] ?? "http://localhost:5101"),
        ("customers", config["Services:Customers"] ?? "http://localhost:5102"),
        ("tickets", config["Services:Tickets"] ?? "http://localhost:5103"),
        ("sla", config["Services:Sla"] ?? "http://localhost:5105"),
        ("channels", config["Services:Channels"] ?? "http://localhost:5201"),
        ("notifications", config["Services:Notifications"] ?? "http://localhost:5202")
    };

    var services = new Dictionary<string, object>();
    var overall = "ok";
    foreach (var (name, baseUrl) in probes)
    {
        try
        {
            var response = await client.GetAsync($"{baseUrl.TrimEnd('/')}/health");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<ServiceHealthStatus>();
                services[name] = body ?? new ServiceHealthStatus(name, "ok");
            }
            else
            {
                overall = "degraded";
                services[name] = new ServiceHealthStatus(name, $"down:{response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            overall = "degraded";
            services[name] = new { service = name, status = "down", error = ex.GetType().Name };
        }
    }

    return Results.Ok(new { status = overall, services });
});

app.MapPost("/login", async (DevLoginRequest request, HttpContext http, IHttpClientFactory httpFactory, IConfiguration config) =>
{
    var client = httpFactory.CreateClient("downstream");
    var identity = config["Services:Identity"] ?? "http://localhost:5101";
    using var response = await client.PostAsJsonAsync($"{identity.TrimEnd('/')}/api/identity/token", request);
    if (!response.IsSuccessStatusCode)
    {
        var err = await response.Content.ReadAsStringAsync();
        return Results.Json(
            string.IsNullOrWhiteSpace(err) ? new { error = "Unauthorized" } : JsonSerializer.Deserialize<object>(err),
            statusCode: (int)response.StatusCode);
    }

    var tokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>(jsonOpts);
    if (tokens?.User is null)
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, tokens.User.Id),
        new(ClaimTypes.Name, tokens.User.Name),
        new(ClaimTypes.Email, tokens.User.Email),
        new(ClaimTypes.Role, tokens.User.Role)
    };
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

    http.Response.Cookies.Append(AccessCookie, tokens.AccessToken, TokenCookieOptions(tokens.AccessTokenExpiresAt));
    http.Response.Cookies.Append(RefreshCookie, tokens.RefreshToken, TokenCookieOptions(tokens.RefreshTokenExpiresAt));

    // Browser only receives user profile — tokens stay in httpOnly cookies.
    return Results.Ok(tokens.User);
});

app.MapPost("/api/auth/refresh", async (HttpContext http, IHttpClientFactory httpFactory, IConfiguration config) =>
{
    if (!http.Request.Cookies.TryGetValue(RefreshCookie, out var refresh) || string.IsNullOrWhiteSpace(refresh))
    {
        return Results.Unauthorized();
    }

    var client = httpFactory.CreateClient("downstream");
    var identity = config["Services:Identity"] ?? "http://localhost:5101";
    using var response = await client.PostAsJsonAsync(
        $"{identity.TrimEnd('/')}/api/identity/token/refresh",
        new RefreshTokenRequest(refresh));
    if (!response.IsSuccessStatusCode)
    {
        http.Response.Cookies.Delete(AccessCookie);
        http.Response.Cookies.Delete(RefreshCookie);
        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Unauthorized();
    }

    var tokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>(jsonOpts);
    if (tokens?.User is null)
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, tokens.User.Id),
        new(ClaimTypes.Name, tokens.User.Name),
        new(ClaimTypes.Email, tokens.User.Email),
        new(ClaimTypes.Role, tokens.User.Role)
    };
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    http.Response.Cookies.Append(AccessCookie, tokens.AccessToken, TokenCookieOptions(tokens.AccessTokenExpiresAt));
    http.Response.Cookies.Append(RefreshCookie, tokens.RefreshToken, TokenCookieOptions(tokens.RefreshTokenExpiresAt));
    return Results.Ok(tokens.User);
});

app.MapPost("/logout", async (HttpContext http, IHttpClientFactory httpFactory, IConfiguration config) =>
{
    http.Request.Cookies.TryGetValue(RefreshCookie, out var refresh);
    http.Request.Cookies.TryGetValue(AccessCookie, out var access);
    if (!string.IsNullOrWhiteSpace(refresh) || !string.IsNullOrWhiteSpace(access))
    {
        try
        {
            var client = httpFactory.CreateClient("downstream");
            var identity = config["Services:Identity"] ?? "http://localhost:5101";
            await client.PostAsJsonAsync(
                $"{identity.TrimEnd('/')}/api/identity/token/revoke",
                new RevokeTokenRequest(refresh, access));
        }
        catch
        {
            // best-effort revoke
        }
    }

    http.Response.Cookies.Delete(AccessCookie);
    http.Response.Cookies.Delete(RefreshCookie);
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.NoContent();
});

app.MapGet("/api/session", (ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Ok(new { authenticated = false });
    }

    return Results.Ok(new
    {
        authenticated = true,
        id = user.FindFirstValue(ClaimTypes.NameIdentifier),
        name = user.Identity.Name,
        email = user.FindFirstValue(ClaimTypes.Email),
        role = user.FindFirstValue(ClaimTypes.Role)
    });
});

app.MapReverseProxy();

app.Run();
