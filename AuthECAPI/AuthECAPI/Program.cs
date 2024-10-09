using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
   .ConfigureCORS(builder.Configuration)
   .AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api")
   .MapIdentityApi<AppUser>();
app.MapGroup("/api")
   .MapIdentityUserEndpoints()
   .MapAccountEndpoints()
   .MapAuthorizationDemoEndpoints();

app.Run();



