using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{

    public class OrganiserController : BaseController
    {
        public OrganiserController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Organiser") return RedirectToAction("Login", "Auth");

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

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("UserRole") != "Organiser") return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(string eventName, string description, DateTime eventDate, string location, string distance, string eventType)
        {
            var client = GetApiClient();
            var json = JsonSerializer.Serialize(new { eventName, description, eventDate, location, distance, eventType, eventStatus = "Open" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/events", content);

            if (response.IsSuccessStatusCode) return RedirectToAction("Dashboard");

            ViewBag.Error = "Failed to create event.";
            return View();
        }

        public async Task<IActionResult> ManageCategories(int eventId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/events/{eventId}/categories");
            var categories = new List<JsonElement>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                categories = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
            }

            ViewBag.EventId = eventId;
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, string categoryName, string categoryType, decimal entryFee)
        {
            var client = GetApiClient();
            var json = JsonSerializer.Serialize(new { categoryName, categoryType, entryFee });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync($"/api/events/{eventId}/categories", content);
            return RedirectToAction("ManageCategories", new { eventId = eventId });
        }
    }
}