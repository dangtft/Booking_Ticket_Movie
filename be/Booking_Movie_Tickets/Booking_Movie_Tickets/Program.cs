using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Users;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSignalR();
// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()//WithOrigins("http://localhost:4200")//SetIsOriginAllowed(host => true)
            .AllowAnyHeader()
            .AllowAnyMethod();
            //.AllowCredentials();

    });
});

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Đăng ký cơ sở dữ liệu
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDbConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<BookingDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IAgeRatingService, AgeRatingService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMovieMediaService, MovieMediaService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAgeRatingService, AgeRatingService>();
builder.Services.AddScoped<IExtraService, ExtraService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddSingleton<IVnPayService, VnPayService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DocumentTitle = "Booking Movie Tickets API";
    c.InjectStylesheet("https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-material.css");
    //https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-dark.css
    //https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-monokai.css
    //https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-flattop.css
    //https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-material.css
    //https://cdn.jsdelivr.net/npm/swagger-ui-themes/themes/3.x/theme-muted.css
    c.DisplayRequestDuration(); 
    c.DefaultModelExpandDepth(2); 
    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
    c.OAuthUsePkce();
    c.OAuthAppName("Booking API");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseSession();
app.UseCors("AllowAnyOrigin");
//app.MapHub<PaymentHub>("/paymentHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
