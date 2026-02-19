using Crops_Price_Tracker.Infrastructure;
using Crops_Price_Tracker.Services;
using Crops_Price_Tracker.Settings;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

/* ================= CONFIGURATION ================= */
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

//builder.Services.Configure<DateRangeSettings>(
//    builder.Configuration.GetSection("DateRangeSettings"));

/* ================= SERVICES ================= */
builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<AlertServices>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* ================= CORS ================= */
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:7085") // Frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

/* ================= JWT AUTH ================= */
//var jwtKey = builder.Configuration["Jwt:Key"];

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey =
//                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

//            // 🔑 IMPORTANT: matches claim name in token
//            NameClaimType = "userId"
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnAuthenticationFailed = context =>
//            {
//                Console.WriteLine($"[JWT ERROR] {context.Exception.Message}");
//                return Task.CompletedTask;
//            },
//            OnTokenValidated = context =>
//            {
//                Console.WriteLine($"[JWT OK] User: {context.Principal.Identity?.Name}");
//                return Task.CompletedTask;
//            }
//        };
//    });

//builder.Services.AddAuthorization();

/* ================= BUILD APP ================= */
var app = builder.Build();

/* ================= MIDDLEWARE ================= */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

//app.UseAuthentication(); // ✅ MUST be before authorization
//app.UseAuthorization();

app.MapControllers();

app.Run();
