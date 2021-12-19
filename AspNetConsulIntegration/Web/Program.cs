using AspNetConsulIntegration;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "wwwroot")
});

builder.Services.AddSingleton<IConsulClient, ConsulClient>(serviceProvider => new ConsulClient());

// Load config from Consul Key-Value store
builder.Services.AddSingleton<IAppConfig>(serviceProvider =>
{
    var _consulClient = new ConsulClient();

    QueryResult<KVPair> queryResult = _consulClient.KV.Get("appsettings/qa4/web").Result;

    if (queryResult.Response == null)
    {
        return new AppConfig();
    }

    var decodedResponse = Encoding.UTF8.GetString(queryResult.Response.Value);
    
    var configOption = System.Text.Json.JsonSerializer.Deserialize<AppConfig>(decodedResponse) ?? new AppConfig();

    return configOption;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();