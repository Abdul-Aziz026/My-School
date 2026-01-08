using Application.Common.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using YourApp.ApiTests.Fixtures;

namespace Tests.My_School.ApiTests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    public CustomWebApplicationFactory<Program> Factory { get; private set; }
    public HttpClient Client { get; private set; } = null!;
    public string baseUrl = "http://localhost:5000";

    public async Task InitializeAsync()
    {
        Factory = new CustomWebApplicationFactory<Program>();
        await Factory.InitializeAsync();
        Client = Factory.CreateClient();
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
        return " token ";
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
                throw new UnauthorizedAccessException("Unauthorized - authentication required.");

            case HttpStatusCode.Forbidden:    // 403
                throw new UnauthorizedAccessException("Forbidden - insufficient permissions.");

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

        // Only build query string if request is provided
        if (request != null)
        {
            // Convert DTO properties to query string
            var queryParams = new Dictionary<string, string>();

            foreach (var prop in typeof(TRequest).GetProperties())
            {
                var value = prop.GetValue(request);
                if (value != null)
                {
                    // Lowercase booleans for model binding
                    if (value is bool b)
                        queryParams[prop.Name] = b.ToString().ToLower();
                    else
                        queryParams[prop.Name] = value.ToString()!;
                }
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

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await Client.PostAsJsonAsync(url, data);

        // Handle response status
        await HandleHttpResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        var response = await Client.PutAsJsonAsync(url, data);

        // Handle response status
        await HandleHttpResponseAsync(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);

        // Handle response status
        await HandleHttpResponseAsync(response);

        return response;
    }
    #endregion
}