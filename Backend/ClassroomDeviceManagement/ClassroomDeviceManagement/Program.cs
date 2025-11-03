using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Repositories.Implements;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Implements;
using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Cấu hình JWT từ appsettings.json
// =====================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

// =====================
// Đăng ký Authentication dùng JWT Bearer
// =====================
builder.Services.AddAuthentication(options =>
{
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

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// =====================
// Thêm Authorization
// =====================
builder.Services.AddAuthorization();

// Add services to the container.

// CORS = Cross-Origin Resource Sharing
// Khi frontend gửi request từ domain khác server, trình duyệt sẽ block theo "Same-Origin Policy"
// Dùng middleware này để cho phép frontend gọi API
// .AllowAnyOrigin(): cho phép mọi domain (dev) / có thể thay bằng .WithOrigins("http://localhost:5173") cho an toàn
// .AllowAnyHeader(): cho phép gửi mọi header
// .AllowAnyMethod(): cho phép GET/POST/PUT/DELETE
builder.Services.AddCors(option =>          // Thêm dịch vụ CORS vào container
{
    option.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});


// Đăng ký các class helper
builder.Services.AddScoped<IDbManager, DbManager>();

// Đăng ký Repository
builder.Services.AddScoped<IDeviceCategoryRepository, DeviceCategoryRepository>();
builder.Services.AddScoped<IDeviceModelRepository, DeviceModelRepository>();
builder.Services.AddScoped<IDeviceInstanceRepository, DeviceInstanceRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IBorrowRequestRepository, BorrowRequestRepository>();

// Đăng ký Services
builder.Services.AddScoped<IDeviceCategoryService, DeviceCategoryService>();
builder.Services.AddScoped<IDeviceModelService, DeviceModelService>();
builder.Services.AddScoped<IDeviceInstanceService,  DeviceInstanceService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBorrowRequestService,  BorrowRequestService>();

// Đăng ký Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}


// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
