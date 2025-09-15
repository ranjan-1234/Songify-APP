using Singer.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Add Controllers and HttpClient for API calls
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Optional: Database helper singleton
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// Error handling and HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP → HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing must come before UseCors and UseAuthorization
app.UseRouting();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Singer API V1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

// CORS
app.UseCors("AllowReactApp");

// Authorization middleware (even if not used now)
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// MVC routing (optional for pages)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
