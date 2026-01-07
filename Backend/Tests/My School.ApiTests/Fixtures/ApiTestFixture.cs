
using Microsoft.AspNetCore.WebUtilities;
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


    #region get users by paged
    public async Task<TResponse?> GetAsync<TRequest, TResponse>(TRequest request, string route)
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
        // Build full URL
        var url = QueryHelpers.AddQueryString(route, queryParams);

        // Call controller
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        // Deserialize response
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await Client.PostAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        var response = await Client.PutAsJsonAsync(url, data);
        return response;
    }
    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);
        return response;
    }
    #endregion
}
