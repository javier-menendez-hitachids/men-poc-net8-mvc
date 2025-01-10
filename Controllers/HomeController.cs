using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MenulioPocMvc.Models;
using Contentful.Core;
using System.ComponentModel.DataAnnotations;
using MenulioPocMvc.CustomerApi.Interfaces;
using MenulioPocMvc.CustomerApi.Services.Interface;

namespace MenulioPocMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IContentfulClient _client;
    private readonly ICustomerService _customerService;

    public HomeController(ILogger<HomeController> logger, IContentfulClient client, ICustomerService customerService)
    {
        _logger = logger;
        _client = client;
        _customerService = customerService;
    }

    public async Task<IActionResult> Index()
    {
        var cards = await _client.GetEntries<CreditCard>();
        var guid = "6a9db0d9-fde8-4a0a-8bcb-c28a67df81db";
        Guid.TryParse(guid, out var guidValue);
        var customer = await _customerService.GetCustomer(guidValue);
        return View(cards);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
