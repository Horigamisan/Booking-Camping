using FinalProject_API.Models;
using FinalProject_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using FinalProject_API.Controllers;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FinalProject_SOAContext>
    (options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("FinalProjectConnectSQL")), ServiceLifetime.Singleton);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddHostedService<BookingCancellationService>();



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //có kiểm tra Issuer (default true)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //có kiểm tra Audience (default true)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        //Đảm bảo phải có thời gian hết hạn trong token
        RequireExpirationTime = true,
        ValidateLifetime = true,
        //Chỉ ra key sử dụng trong token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RequireSignedTokens = true
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinalProjectAPI",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};
StripeConfiguration.ApiKey = "sk_test_51MzJdpGU9Onc63X1NQywO67PE7SSyfWj3iEP4RMKy4QiVhoLvoAA3sychOCdE3bC7JVc84ibBLObId9I5GmEGUTz003k7NOcZz";
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
