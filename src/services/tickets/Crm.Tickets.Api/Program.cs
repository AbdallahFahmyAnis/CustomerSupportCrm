using Crm.Tickets.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTicketsApi();

var app = builder.Build();
app.UseTicketsApi();
app.Run();

public partial class Program;
