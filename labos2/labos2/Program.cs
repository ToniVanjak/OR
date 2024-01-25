using labos2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Auth0 Auth, reading the app settings above
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

builder.Services.AddSingleton<IMongoClient>(provider => new MongoClient("mongodb://localhost:27017"));
builder.Services.AddScoped<labos2.Data.DbContext>();

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options => {
    options.LoginPath = "/api/igraci/login";
    options.LogoutPath = "/api/igraci/logout";
    options.Events.OnRedirectToLogin = context => {
        context.Response.Headers["Location"] = context.RedirectUri;
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapFallbackToFile("index.html");
});

app.Run();
