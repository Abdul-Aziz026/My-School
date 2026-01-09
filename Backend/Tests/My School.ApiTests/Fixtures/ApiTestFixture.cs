using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit.Sdk;
using YourApp.ApiTests.Fixtures;

namespace Tests.My_School.ApiTests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    public TestOutputHelper printer { get; } = new();
    public CustomWebApplicationFactory<Program> Factory { get; private set; }
    public HttpClient Client { get; private set; } = null!;

    // Store cookies to maintain session state across requests
    private CookieContainer _cookieContainer = new();

    public async Task InitializeAsync()
    {
        Factory = new CustomWebApplicationFactory<Program>();
        await Factory.InitializeAsync();

        // Create client with cookie support using factory
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true // Let the factory handle cookies
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    #region Auth Token Management
    public string GetAuthToken()
    {
        return Client.DefaultRequestHeaders.Authorization?.Parameter ?? string.Empty;
    }

    public void SetAuthToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
    #endregion

    #region Cookie Management
    public void SetCookie(string name, string value, string path = "/")
    {
        // Remove existing Cookie header first
        Client.DefaultRequestHeaders.Remove("Cookie");

        // Add new cookie
        Client.DefaultRequestHeaders.Add("Cookie", $"{name}={value}");
    }

    public string? GetCookie(string name)
    {
        // Extract cookies from the response headers after a request
        // This is a limitation - cookies are managed internally by the factory
        // For testing purposes, cookies from responses are automatically stored
        return null;
    }

    public void ClearCookies()
    {
        // Simply remove the Cookie header
        Client.DefaultRequestHeaders.Remove("Cookie");
    }

    public void ClearAllCookies()
    {
        ClearCookies();
    }
    #endregion

    #region HTTP Response Handling
    private async Task HandleHttpResponseAsync(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            // Success cases
            case HttpStatusCode.OK:           // 200 - GET
            case HttpStatusCode.Created:      // 201 - POST
            case HttpStatusCode.NoContent:    // 204 - DELETE/PUT
                break;

            // Client errors (4xx)
            case HttpStatusCode.BadRequest:   // 400
                var badRequestContent = await response.Content.ReadAsStringAsync();
                throw new ArgumentException($"Bad Request: {badRequestContent}");

            case HttpStatusCode.Unauthorized: // 401
                var unauthorizedContent = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException($"Unauthorized: {unauthorizedContent}");

            case HttpStatusCode.Forbidden:    // 403
                var forbiddenContent = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException($"Forbidden: {forbiddenContent}");

            case HttpStatusCode.NotFound:     // 404
                var notFoundContent = await response.Content.ReadAsStringAsync();
                throw new NotFoundException($"Not Found: {notFoundContent}");

            case HttpStatusCode.Conflict:     // 409
                var conflictContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Conflict: {conflictContent}");

            case HttpStatusCode.RequestTimeout: // 408
                throw new TimeoutException("Request timed out.");

            // Server errors (5xx)
            case HttpStatusCode.InternalServerError: // 500
                var serverErrorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Internal Server Error: {serverErrorContent}");

            default:
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request failed with status {(int)response.StatusCode} ({response.StatusCode}): {content}");
        }
    }
    #endregion

    #region HTTP Methods
    public async Task<TResponse?> GetAsync<TRequest, TResponse>(TRequest request, string route)
    {
        var url = route;

        // Only build query string if request is provided and not null
        if (request != null)
        {
            // Convert DTO properties to query string
            var queryParams = new Dictionary<string, string>();

            foreach (var prop in typeof(TRequest).GetProperties())
            {
                var value = prop.GetValue(request);
                if (value == null)
                    continue; // skip null

                if (value is string s && string.IsNullOrWhiteSpace(s))
                    continue; // skip empty string

                if (value is bool b)
                    queryParams[prop.Name] = b.ToString().ToLower();
                else
                    queryParams[prop.Name] = value.ToString()!;
            }

            if (queryParams.Any())
            {
                url = QueryHelpers.AddQueryString(route, queryParams!);
            }
        }

        // Call controller
        var response = await Client.GetAsync(url);

        // Handle response status
        await HandleHttpResponseAsync(response);

        // Deserialize response
        var result = await response.Content.ReadFromJsonAsync<TResponse>();
        return result;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string route)
    {
        // Call controller
        var response = await Client.GetAsync(route);

        // Handle response status
        await HandleHttpResponseAsync(response);

        // Deserialize response
        var result = await response.Content.ReadFromJsonAsync<TResponse>();
        return result;
    }


    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await Client.PostAsJsonAsync(url, data);

        // Handle response status
        await HandleHttpResponseAsync(response);

        // Handle 204 No Content responses
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PostAsync<TResponse>(string url)
    {
        var response = await Client.PostAsync(url, null);

        // Handle response status
        await HandleHttpResponseAsync(response);

        // Handle 204 No Content responses
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        var response = await Client.PutAsJsonAsync(url, data);

        // Handle response status
        await HandleHttpResponseAsync(response);

        return response;
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await Client.PutAsJsonAsync(url, data);

        // Handle response status
        await HandleHttpResponseAsync(response);

        // Handle 204 No Content responses
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);

        // Handle response status
        await HandleHttpResponseAsync(response);

        return response;
    }
    #endregion

    #region Test Helpers
    /// <summary>
    /// Resets the test fixture to a clean state - clears auth tokens and cookies
    /// </summary>
    public void Reset()
    {
        ClearAuthToken();
        ClearCookies();
    }

    /// <summary>
    /// Creates an authenticated client by registering and logging in a test user
    /// </summary>
    public async Task<string> CreateAuthenticatedUserAsync(
        string email = "testuser@example.com",
        string username = "testuser",
        string password = "Password123!")
    {
        var registerDto = new
        {
            Email = email,
            UserName = username,
            Password = password,
            PhoneNumber = "1234567890"
        };

        var response = await PostAsync<object, dynamic>("/api/auth/register", registerDto);
        var token = response?.AccessToken?.ToString() ?? string.Empty;

        SetAuthToken(token);
        return token;
    }
    #endregion
}