using System.Text;
using API.Extensions;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger(builder.Environment);

//builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        var keyByteArray = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]);

        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyByteArray),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = string.Join(", ", builder.Configuration["Jwt:Audience"]),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = false
        };
        
        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                
                logger.LogDebug("""
                                JWT Authentication Failed
                                Exception: {Exception}
                                StackTrace: {StackTrace}
                                Token: {Token}
                                """, 
                    context.Exception.Message,
                    context.Exception.StackTrace,
                    context.Request.Headers["Authorization"]);
                
                return Task.CompletedTask;
            },
            
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                
                logger.LogDebug("""
                                JWT Challenge
                                Error: {Error}
                                ErrorDescription: {ErrorDescription}
                                ErrorUri: {ErrorUri}
                                """,
                    context.Error,
                    context.ErrorDescription,
                    context.ErrorUri);
                
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

var loggerProgram = app.Services.GetRequiredService<ILogger<Program>>();

app.UseSwagger(app.Environment);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

loggerProgram.LogInformation("Started: https://localhost:7276/swagger/");

app.Run();