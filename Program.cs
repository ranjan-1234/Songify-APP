using Microsoft.EntityFrameworkCore;
using Singer.Helpers;
using Singer.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// 1️⃣ Read connection string safely
// -------------------------------
string connectionString;
try
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
        throw new Exception("Connection string 'DefaultConnection' is missing in appsettings.json.");

    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
    if (string.IsNullOrEmpty(mysqlPassword))
        throw new Exception("MYSQL_PASSWORD environment variable is not set on Render.");

    // Replace placeholder in appsettings.json with actual password
    connectionString = connectionString.Replace("${MYSQL_PASSWORD}", mysqlPassword);
}
catch (Exception ex)
{
    Console.WriteLine($"Error reading connection string: {ex.Message}");
    throw; // Stop app if connection string is invalid
}

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
builder.Services.AddControllers(); // API-only
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
    // Use JSON error responses for SPA
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.ContentType = "application/json";
            var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var error = feature?.Error;
            await context.Response.WriteAsJsonAsync(new { message = error?.Message });
        });
    });
}
else
{
    app.UseDeveloperExceptionPage();
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
