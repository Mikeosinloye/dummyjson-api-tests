using DummyJson.ApiTests.Models;
using RestSharp;

namespace DummyJson.ApiTests.Support;

public sealed class ScenarioState
{
    public RestResponse? LastResponse { get; set; }

    public LoginResponse? LoginResult { get; set; }
    public AddUserRequest? UserToCreate { get; set; }
    public Product? OriginalProduct { get; set; }
    public AddCartRequest? CartRequest { get; set; }

    public RestResponse RequireLastResponse() =>
        LastResponse ?? throw new InvalidOperationException("No HTTP request has been sent in this scenario yet.");

    public T RequireData<T>() where T : class
    {
        var response = RequireLastResponse();
        if (response is not RestResponse<T> typed)
            throw new InvalidOperationException(
                $"Last response was not requested as {typeof(T).Name}; check the step order.");
        return typed.Data ?? throw new InvalidOperationException(
            $"Response body could not be deserialised to {typeof(T).Name}. Body: {response.Content}");
    }
}

public sealed class AuthTokenStore
{
    private string? _accessToken;

    public bool HasToken => !string.IsNullOrWhiteSpace(_accessToken);

    public void Store(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Refusing to store an empty access token.", nameof(accessToken));
        _accessToken = accessToken;
    }

    public string AccessToken =>
        _accessToken ?? throw new InvalidOperationException("No access token stored — authenticate first.");
}
