using Crm.Sla.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSlaApi();

var app = builder.Build();
app.UseSlaApi();
app.Run();

public partial class Program;
