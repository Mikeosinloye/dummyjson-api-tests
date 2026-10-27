using Microsoft.Extensions.Configuration;

namespace DummyJson.ApiTests.Config;

public sealed class ApiSettings
{
    public string BaseUrl { get; init; } = string.Empty;
    public int TimeoutSeconds { get; init; } = 30;
    public int TokenExpiresInMins { get; init; } = 30;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public static class TestConfiguration
{
    private static readonly Lazy<ApiSettings> LazySettings = new(Load);

    public static ApiSettings Api => LazySettings.Value;

    private static ApiSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables(prefix: "DUMMYJSON_")
            .Build();

        var settings = configuration.GetSection("Api").Get<ApiSettings>()
                       ?? throw new InvalidOperationException("Missing 'Api' section in appsettings.json.");

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            throw new InvalidOperationException("Api:BaseUrl is not configured.");

        return settings;
    }
}
