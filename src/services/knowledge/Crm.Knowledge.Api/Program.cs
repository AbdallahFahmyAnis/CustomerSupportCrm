using Crm.Knowledge.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKnowledgeApi();

var app = builder.Build();
app.UseKnowledgeApi();
app.Run();

public partial class Program;
