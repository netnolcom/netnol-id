using Netnol.Identity.Service.Utilities;
using Netnol.Identity.Service.Infrastructure.Configuration;
using Scalar.AspNetCore;

EnvironmentInitializer.Initialize();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference("/");
app.Run();