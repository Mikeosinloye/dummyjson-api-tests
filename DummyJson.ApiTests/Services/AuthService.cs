using DummyJson.ApiTests.Client;
using DummyJson.ApiTests.Models;
using RestSharp;

namespace DummyJson.ApiTests.Services;

public sealed class AuthService(ApiClient client)
{
    public Task<RestResponse<LoginResponse>> LoginAsync(string username, string password, int expiresInMins)
    {
        var request = new RestRequest("auth/login", Method.Post)
            .AddJsonBody(new LoginRequest(username, password, expiresInMins));
        return client.ExecuteAsync<LoginResponse>(request);
    }

    public Task<RestResponse<User>> GetCurrentUserAsync(string accessToken)
    {
        var request = new RestRequest("auth/me")
            .AddHeader("Authorization", $"Bearer {accessToken}");
        return client.ExecuteAsync<User>(request);
    }
}
