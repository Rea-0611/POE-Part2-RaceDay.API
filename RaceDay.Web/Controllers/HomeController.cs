using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace RaceDay.Web.Controllers { 

public class HomeController : BaseController
{
    public HomeController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

    public async Task<IActionResult> Index()
    {
        var client = GetApiClient();
        var response = await client.GetAsync("/api/events");
        var events = new List<JsonElement>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            events = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
        }
        return View(events);
    }

    public async Task<IActionResult> EventDetails(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/events/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var ev = JsonDocument.Parse(json).RootElement;
        return View(ev);
    }
}}