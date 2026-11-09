using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

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

        var response = await client.PostAsync("auth/login", content);

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
    public async Task<IActionResult> Register(string email, string password, string fullName, string role, string? organisationName, string? contactNumber)
    {
        var client = GetApiClient();
        var json = JsonSerializer.Serialize(new { email, password, fullName, role, organisationName, contactNumber });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ViewBag.Error = "Registration failed. Email might be taken.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
