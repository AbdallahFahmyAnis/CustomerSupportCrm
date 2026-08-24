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
            }

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();
app.UseCorrelationId();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", async (IHttpClientFactory httpFactory, IConfiguration config) =>
{
    var client = httpFactory.CreateClient("downstream");
    var probes = new (string Name, string Url)[]
    {
        ("identity", config["Services:Identity"] ?? "http://localhost:5101"),
        ("customers", config["Services:Customers"] ?? "http://localhost:5102"),
        ("tickets", config["Services:Tickets"] ?? "http://localhost:5103"),
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
    using var response = await client.PostAsJsonAsync($"{identity.TrimEnd('/')}/api/identity/dev-login", request);
    if (!response.IsSuccessStatusCode)
    {
        return Results.Unauthorized();
    }

    var user = await response.Content.ReadFromJsonAsync<DevUserDto>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
    if (user is null)
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Name, user.Name),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role)
    };
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

    return Results.Ok(user);
});

app.MapPost("/logout", async (HttpContext http) =>
{
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
