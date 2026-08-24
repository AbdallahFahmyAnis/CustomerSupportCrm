using Crm.Identity.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityApi();

var app = builder.Build();
app.UseIdentityApi();
app.Run();

public partial class Program;
