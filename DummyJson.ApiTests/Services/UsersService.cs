using DummyJson.ApiTests.Client;
using DummyJson.ApiTests.Models;
using RestSharp;

namespace DummyJson.ApiTests.Services;

public sealed class UsersService(ApiClient client)
{
    public Task<RestResponse<UsersResponse>> FilterAsync(string key, string value, string? select = null)
    {
        var request = new RestRequest("users/filter")
            .AddQueryParameter("key", key)
            .AddQueryParameter("value", value)
            .AddQueryParameter("limit", "0");
        if (select is not null)
            request.AddQueryParameter("select", select);
        return client.ExecuteAsync<UsersResponse>(request);
    }

    public Task<RestResponse<UsersResponse>> GetAllAsync(string? select = null)
    {
        var request = new RestRequest("users").AddQueryParameter("limit", "0");
        if (select is not null)
            request.AddQueryParameter("select", select);
        return client.ExecuteAsync<UsersResponse>(request);
    }

    public Task<RestResponse<User>> AddAsync(AddUserRequest user)
    {
        var request = new RestRequest("users/add", Method.Post).AddJsonBody(user);
        return client.ExecuteAsync<User>(request);
    }
}
