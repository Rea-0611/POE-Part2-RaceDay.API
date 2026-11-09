using Microsoft.AspNetCore.Mvc;


namespace RaceDay.Web.Controllers;

public class BaseController : Controller
{
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly string _apiBaseUrl = "https://localhost:7268/api";
    public BaseController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // This helper automatically attaches the JWT token to every API request
    protected HttpClient GetApiClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_apiBaseUrl);

        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }
}