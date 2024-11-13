using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poochatting;
using Poochatting.Authorization;
using Poochatting.ChatHub;
using Poochatting.DbContext;
using Poochatting.DbContext.Entities;
using Poochatting.Middleware;
using Poochatting.Models;
using Poochatting.Models.Queries;
using Poochatting.Models.Validators;
using Poochatting.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
    cfg.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatHub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ValidateModelAttribute());
});

builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddDbContext<MessageDbContext>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSignalR();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountContextService, AccountContextService>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<MessageQueryParams>, MessageQueryValidator>();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("All", builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseCors("All");
// Configure the HTTP request pipeline.

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/chatHub");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message API");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
