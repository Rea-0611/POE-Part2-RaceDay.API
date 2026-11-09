
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers {

    public class ParticipantController : BaseController
    {
        public ParticipantController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant") return RedirectToAction("Login", "Auth");

            var client = GetApiClient();
            var json = JsonSerializer.Serialize(new { paymentStatus = "Pending" });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync($"/api/events/{eventId}/categories/{categoryId}/enrol", content);
            return RedirectToAction("MyEnrolments");
        }

        public async Task<IActionResult> MyEnrolments()
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant") return RedirectToAction("Login", "Auth");

            var client = GetApiClient();
            var response = await client.GetAsync("/api/participants/me/enrolments");
            var enrolments = new List<JsonElement>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                enrolments = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
            }
            return View(enrolments);
        }

        public async Task<IActionResult> MyResults()
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant") return RedirectToAction("Login", "Auth");

            var client = GetApiClient();
            var response = await client.GetAsync("/api/participants/me/results");
            var results = new List<JsonElement>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                results = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
            }
            return View(results);
        }
        // 1. Show profile
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant") return RedirectToAction("Login", "Auth");

            var client = GetApiClient();
            var response = await client.GetAsync("/api/users/profile");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonDocument.Parse(json).RootElement;
            return View(profile);
        }

        // 2. Update profile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string contactNumber)
        {
            var client = GetApiClient();
            var json = JsonSerializer.Serialize(new { fullName, contactNumber });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync("/api/users/profile", content);

            // Update session name so navbar updates immediately
            HttpContext.Session.SetString("UserName", fullName);

            return RedirectToAction("Profile");
        }
    }
}