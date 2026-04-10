using Netnol.Identity.Service.Application.Services;
using Netnol.Identity.Service.Domain.Repositories;
using Netnol.Identity.Service.Infrastructure.Configuration;
using Netnol.Identity.Service.Infrastructure.Data;
using Netnol.Identity.Service.Infrastructure.Extensions;
using Netnol.Identity.Service.Infrastructure.Repositories;
using Scalar.AspNetCore;

EnvironmentInitializer.Initialize();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DatabaseContext>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddCaching();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference("/");
app.Run();