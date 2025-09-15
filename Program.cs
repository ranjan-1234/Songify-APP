using Microsoft.EntityFrameworkCore;
using Singer.Helpers;
using Singer.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// 1️⃣ Read connection string
// -------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Replace placeholder with environment variable (Render secret)
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
if (string.IsNullOrEmpty(mysqlPassword))
{
    throw new Exception("MYSQL_PASSWORD environment variable is not set!");
}

// Replace placeholder {password_placeholder} in appsettings.json
connectionString = connectionString.Replace("${MYSQL_PASSWORD}", mysqlPassword);

// -------------------------------
// 2️⃣ CORS configuration
// -------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                    "https://my-spongify.netlify.app",
                    "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// -------------------------------
// 3️⃣ Add services
// -------------------------------
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core + DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 34))));

// Optional: Database helper singleton
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// -------------------------------
// 4️⃣ Middleware
// -------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Singer API V1");
    c.RoutePrefix = string.Empty;
});

// CORS
app.UseCors("AllowReactApp");

// Authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
