using AutoMapper;
using LearnAPI.Container;
using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<LearndataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var _authKey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero

    };
});

builder.Services.AddControllers();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IRefreshHandler, RefreshHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();


var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddCors(p => p.AddPolicy("corspolicy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddPolicy("corspolicy1", builder =>
{
    builder.WithOrigins("https://domain3.com").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddDefaultPolicy(builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit = 0 ;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode=401);

string logPath = builder.Configuration.GetSection("Logging:LogPath").Value;

var _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(logPath)
                .CreateLogger();

builder.Logging.AddSerilog(_logger);

var _jwtsettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.Configure<JwtSettings>(_jwtsettings);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); 

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
