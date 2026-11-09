using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay.Web.Controllers;

public class AuthController : BaseController
{
    public AuthController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var client = GetApiClient();
        var json = JsonSerializer.Serialize(new { email, password });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(responseString).RootElement;

            // Store token and role in Session
            HttpContext.Session.SetString("JwtToken", data.GetProperty("token").GetString()!);
            HttpContext.Session.SetString("UserRole", data.GetProperty("role").GetString()!);
            HttpContext.Session.SetString("UserName", data.GetProperty("fullName").GetString()!);

            // Redirect based on role
            if (data.GetProperty("role").GetString() == "Organiser")
                return RedirectToAction("Dashboard", "Organiser");
            else
                return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid email or password.";
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string fullName, string role, string? organisationName, string? contactNumber, DateTime? dateOfBirth)
    {
        var client = GetApiClient();

        var payload = new
        {
            email,
            password,
            fullName,
            role,
            organisationName,
            contactNumber,
            dateOfBirth
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            // Registration successful! Send them to the Login page.
            return RedirectToAction("Login");
        }

        // If it fails, read the error
        var errorDetails = await response.Content.ReadAsStringAsync();
        ViewBag.Error = $"Error {(int)response.StatusCode}: {errorDetails}";

        return View();
    }
}
