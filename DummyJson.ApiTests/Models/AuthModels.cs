namespace DummyJson.ApiTests.Models;

public sealed record LoginRequest(string Username, string Password, int ExpiresInMins);

public sealed class LoginResponse
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
