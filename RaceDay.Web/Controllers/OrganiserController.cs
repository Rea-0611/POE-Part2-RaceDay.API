using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers { 

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

    // --- EVENT CRUD ---
    [HttpGet]
    public IActionResult CreateEvent() => View();

    [HttpPost]
    public async Task<IActionResult> CreateEvent(string eventName, string description, DateTime eventDate, string location, string distance, string eventType)
    {
        var client = GetApiClient();
        var json = JsonSerializer.Serialize(new { eventName, description, eventDate, location, distance, eventType, eventStatus = "Open" });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/events", content);
        if (response.IsSuccessStatusCode) return RedirectToAction("Dashboard");

        ViewBag.Error = "Failed to create event. Check your inputs.";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/events/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var ev = JsonDocument.Parse(json).RootElement;
        return View(ev);
    }

    [HttpPost]
    public async Task<IActionResult> EditEvent(int id, string eventName, string description, DateTime eventDate, string location, string distance, string eventType, string eventStatus)
    {
        var client = GetApiClient();
        var json = JsonSerializer.Serialize(new { eventName, description, eventDate, location, distance, eventType, eventStatus });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await client.PutAsync($"/api/events/{id}", content);
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var client = GetApiClient();
        await client.DeleteAsync($"/api/events/{id}");
        return RedirectToAction("Dashboard");
    }

    // --- CATEGORIES ---
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
        return RedirectToAction("ManageCategories", new { eventId });
    }

    // --- ENROLMENTS & PAYMENTS ---
    public async Task<IActionResult> EventEnrolments(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/events/{id}/enrolments");
        var enrolments = new List<JsonElement>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            enrolments = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
        }

        ViewBag.EventId = id;
        return View(enrolments);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsPaid(int enrolmentId, int eventId)
    {
        var client = GetApiClient();
        var json = JsonSerializer.Serialize(new { paymentStatus = "Paid" });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await client.PutAsync($"/api/enrolments/{enrolmentId}", content);
        return RedirectToAction("EventEnrolments", new { id = eventId });
    }

    // --- RESULTS ---
    public async Task<IActionResult> EventResults(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/events/{id}/results");
        var results = new List<JsonElement>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            results = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
        }
        ViewBag.EventId = id;
        return View(results);
    }

    [HttpGet]
    public IActionResult CaptureResult(int enrolmentId)
    {
        ViewBag.EnrolmentId = enrolmentId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int overallPosition, int categoryPosition)
    {
        var client = GetApiClient();
        var timeSpan = TimeSpan.Parse(finishTime);
        var json = JsonSerializer.Serialize(new { finishTime = timeSpan, overallPosition, categoryPosition });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await client.PostAsync($"/api/enrolments/{enrolmentId}/results", content);
        return RedirectToAction("Dashboard");
    }
}
}