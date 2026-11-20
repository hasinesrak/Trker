using DotNetEnv;
using trker.Data;

// Load environment variables from .env before the configuration is built.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Keep the Mongo connection details in environment variables so secrets stay out of appsettings.
var mongoUri = Environment.GetEnvironmentVariable("MONGODB_URI");
if (!string.IsNullOrWhiteSpace(mongoUri))
{
    builder.Configuration["MongoDb:ConnectionString"] = mongoUri;
}

var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
if (!string.IsNullOrWhiteSpace(mongoDatabaseName))
{
    builder.Configuration["MongoDb:DatabaseName"] = mongoDatabaseName;
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMongoDb(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    await app.VerifyMongoDbConnectionAsync();
}

app.Run();
