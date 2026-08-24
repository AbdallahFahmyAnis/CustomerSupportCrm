using Crm.Customers.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomersApi();

var app = builder.Build();
app.UseCustomersApi();
app.Run();

public partial class Program;
