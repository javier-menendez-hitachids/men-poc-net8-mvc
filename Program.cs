using Contentful.AspNetCore;
using MenulioPocMvc;
using MenulioPocMvc.CustomerApi;
using MenulioPocMvc.CustomerApi.Interfaces;
using MenulioPocMvc.CustomerApi.Services;
using MenulioPocMvc.CustomerApi.Services.Interface;
using MenulioPocMvc.Telemetry;
using MenulioPocMvc.Telemetry.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddContentful(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<HttpRequestClient>(client =>
{
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", builder.Configuration["CustomerApi.SubscriptionKey"]);
});

builder.Services.AddSingleton<Configuration>();

builder.Services.AddScoped<IHttpRequestClient, HttpRequestClient>();
builder.Services.AddScoped<IApiCalls, ApiCalls>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITelemetryHelper, TelemetryHelper>();

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

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
