using BrotatoServer.Data;
using BrotatoServer.Data.Game;
using BrotatoServer.Hubs;
using BrotatoServer.SearchEngine;
using BrotatoServer.Services;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

const string API_KEY = "240ea58c-2870-4676-829a-6cbefaf950f8";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<BrotatoServerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BrotatoServerContext") ?? throw new InvalidOperationException("Connection string 'BrotatoServerContext' not found.")));
builder.Services.AddServerSideBlazor();

builder.Services
    .AddScoped<IRunRepository, RunRepository>()
    .AddSingleton<CurrentRun>();
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddSignalR();

builder.Services
    .AddSingleton<TwitchChatService>()
    .AddHostedService(sp => sp.GetRequiredService<TwitchChatService>());
builder.Services.AddSingleton<BrotatoItemEngine>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}


app.Use((context, next) =>
{
    // Check if the request is for an API endpoint
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        // Check if the "ApiKey" header exists and its value matches the expected API key
        if (!context.Request.Headers.TryGetValue("x-api-key", out var headerValue) || headerValue != API_KEY)
        {
            // If the API key is missing or doesn't match, return 401 Unauthorized
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }

    // If the API key is valid or the request is not for an API endpoint, continue to the next middleware
    return next();
});
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<RunsHub>("/runsHub");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<BrotatoServerContext>(); // Replace 'YourDbContext' with your actual DbContext class.
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Handle the exception as needed (e.g., log or display an error message).
        Console.WriteLine("Error occurred while applying migrations: " + ex.Message);
    }
}

app.Run();
