using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthECAPI.Extensions
{
  public static class IdentityExtensions
  {
    public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
    {
      services.AddIdentityApiEndpoints<AppUser>()
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<AppDbContext>();
      return services;
    }

    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
      });
      return services;
    }

    //Auth = Authentication + Authorization
    public static IServiceCollection AddIdentityAuth(
        this IServiceCollection services,
        IConfiguration config)
    {
      services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(y =>
        {
          y.SaveToken = false;
          y.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                config["AppSettings:JWTSecret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
          };
        });
      services.AddAuthorization(options =>
      {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
          .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
          .RequireAuthenticatedUser()
          .Build();

        options.AddPolicy("HasLibraryID", policy => policy.RequireClaim("libraryID"));
        options.AddPolicy("FemalesOnly", policy => policy.RequireClaim("gender", "Female"));
        options.AddPolicy("Under10", policy => policy.RequireAssertion(context =>
        Int32.Parse(context.User.Claims.First(x => x.Type== "age").Value)<10));

      });


      return services;
    }

    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
      app.UseAuthentication();
      app.UseAuthorization();
      return app;
    }

  }
}

