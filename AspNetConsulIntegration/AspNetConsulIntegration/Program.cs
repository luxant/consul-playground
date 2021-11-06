using AspNetConsulIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Host.ConfigureAppConfiguration(configBuilder =>
{
    configBuilder.Sources.Add(new ConsulConfigurationSource());
});

// If want to use extension method instead uncomment this one and comment above
//builder.Configuration.AddConsulConfigProvider();

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


// Extension method to add Consul Config Provider
public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddConsulConfigProvider(
        this IConfigurationBuilder builder)
    {
        return builder.Add(new ConsulConfigurationSource());
    }
}