using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MenulioPocMvc.Models;
using Contentful.Core;
using System.ComponentModel.DataAnnotations;

namespace MenulioPocMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IContentfulClient _client;

    public HomeController(ILogger<HomeController> logger, IContentfulClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        var cards = await _client.GetEntries<CreditCard>();
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
