using Microsoft.EntityFrameworkCore;
using Task1.API.Services;
using Task1.Database;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<Task1Context>(options=>options.UseNpgsql(builder.Configuration.GetConnectionString("Task1Connection")));
builder.Services.AddSingleton<LobbyService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LobbyService>();
app.MapGrpcService<GameService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
