using Contentful.AspNetCore;
using MenulioPocMvc;
using MenulioPocMvc.CustomerApi;
using MenulioPocMvc.CustomerApi.Interfaces;
using MenulioPocMvc.CustomerApi.Services;
using MenulioPocMvc.CustomerApi.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Configuration>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddContentful(builder.Configuration);

builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", builder.Configuration["CustomerApiClient.SubscriptionKey"]);
});

builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

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
