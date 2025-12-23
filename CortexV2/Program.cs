using Cortex.DTOs;
using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var apiBase = builder.Configuration.GetSection("Api").GetValue<string>("BaseUrl") ?? "http://cortex.runasp.net/";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.


// Add services
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
// Add delegating handler to attach JWT from authenticated cookie claims
builder.Services.AddTransient<JwtDelegatingHandler>();
builder.Services.AddScoped<IApiClient, ApiClient>();

// Register ApiClient with HttpClient factory and message handler
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<JwtDelegatingHandler>();

// Configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// DbContext
builder.Services.AddDbContext<HospitalManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<PatientsService>(client =>
{
    client.BaseAddress = new Uri(apiBase); // update to your API base
}).AddHttpMessageHandler<JwtDelegatingHandler>();
builder.Services.AddHttpClient<AppointmentService>(client =>
{
    client.BaseAddress = new Uri(apiBase); // update to your API base
});
builder.Services.AddHttpClient<RoomService>(client =>
{
    client.BaseAddress = new Uri(apiBase); // update to your API base
});
builder.Services.AddHttpClient<BillingService>(client =>
{
    client.BaseAddress = new Uri(apiBase); // update to your API base
});
// JWT Authentication
if (jwtSettings == null)
    throw new Exception("JwtSettings not configured");

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // ✅ Change to true for production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie(options =>
{
    options.Cookie.Name = "authToken";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ For production
}); ;


// Add authorization
builder.Services.AddAuthorization();

// Add controllers with global auth filter
/*builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});*/

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hospital Management API", Version = "v1" });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowProduction",
        builder => builder
            /*.WithOrigins(
                "https://localhost:7190",
                "https://127.0.0.1:7190",
                "http://cortex.runasp.net/" // ✅ Add your production domain
            )*/
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
var app = builder.Build();
app.MapRazorPages();     // Map Razor Pages (Login UI)

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Management API v1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✅ CRITICAL: Correct middleware order
app.UseStaticFiles();    // Serve CSS, JS, images for Razor Pages
app.UseRouting();        // Enable routing
// Use CORS (place this after UseRouting() but before UseAuthorization())
app.UseCors("AllowRazorPages");
// ✅ Use CORS
app.UseCors("AllowProduction");

app.UseAuthentication(); // JWT authentication
app.UseAuthorization();  // Authorization


app.MapControllers();

app.Run();
