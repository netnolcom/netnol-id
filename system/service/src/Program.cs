using Netnol.Identity.Service.Utilities;
using Scalar.AspNetCore;

Service.Initialize();

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