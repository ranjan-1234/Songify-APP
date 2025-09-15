
using Microsoft.EntityFrameworkCore;
using Singer.Helpers;
using Singer.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Read connection string from appsettings.json
// ----------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ----------------------------
// CORS: allow your React frontend
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                    "https://my-spongify.netlify.app", // Netlify frontend
                    "http://localhost:3000")           // optional: local dev
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// ----------------------------
// Add services
// ----------------------------
builder.Services.AddControllers();
builder.Services.AddHttpClient();           // for API calls (Last.fm, etc.)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// Register EF Core + DbContext
// ----------------------------
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);


// Optional: Database helper singleton (if you need manual queries)
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// ----------------------------
// Middleware
// ----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Singer API V1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

// Enable CORS for frontend
app.UseCors("AllowReactApp");

// Authorization middleware (even if not used now)
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Optional MVC route for pages (if needed)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
