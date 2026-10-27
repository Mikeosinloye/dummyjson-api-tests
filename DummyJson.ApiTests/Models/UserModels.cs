namespace DummyJson.ApiTests.Models;

public sealed class User
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public Hair? Hair { get; init; }
    public Address? Address { get; init; }
}

public sealed record AddUserRequest(
    string FirstName,
    string LastName,
    int Age,
    string Gender,
    Hair Hair,
    Address Address);

public sealed class Hair
{
    public string Color { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}

public sealed class Address
{
    [System.Text.Json.Serialization.JsonPropertyName("address")]
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string StateCode { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public Coordinates? Coordinates { get; init; }
    public string Country { get; init; } = string.Empty;
}

public sealed class Coordinates
{
    public decimal? Lat { get; init; }
    public decimal? Lng { get; init; }
}

public sealed class UsersResponse
{
    public List<User> Users { get; init; } = [];
    public int Total { get; init; }
    public int Skip { get; init; }
    public int Limit { get; init; }
}
