
using System.Net.Http.Headers;
using System.Net.Http.Json;
using YourApp.ApiTests.Fixtures;

namespace Tests.My_School.ApiTests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    public CustomWebApplicationFactory<Program> Factory { get; private set; }
    public HttpClient Client { get; private set; } = null!;

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


    #region All api crud operations
    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        var response = await Client.PostAsJsonAsync(url, data);
        return response;
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
