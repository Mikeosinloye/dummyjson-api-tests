using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DummyJson.ApiTests.Config;
using DummyJson.ApiTests.Support;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.Client;

public sealed partial class ApiClient
{
    private static readonly Lazy<RestClient> SharedClient = new(() =>
        new RestClient(new RestClientOptions(TestConfiguration.Api.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(TestConfiguration.Api.TimeoutSeconds),
            ThrowOnAnyError = false
        }));

    private const int MaxLoggedBodyLength = 2000;

    private static readonly JsonSerializerOptions WireFormat = new(JsonSerializerDefaults.Web);

    private readonly IReqnrollOutputHelper _output;
    private readonly HttpLog _httpLog;

    public ApiClient(IReqnrollOutputHelper output, HttpLog httpLog)
    {
        _output = output;
        _httpLog = httpLog;
    }

    public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) where T : class
    {
        var response = await SharedClient.Value.ExecuteAsync<T>(request);
        Log(request, response);
        return response;
    }

    private void Log(RestRequest request, RestResponse response)
    {
        var method = request.Method.ToString().ToUpperInvariant();
        var url = SharedClient.Value.BuildUri(request);

        _output.WriteLine($"{method} {url} -> {(int)response.StatusCode} {response.StatusCode}");

        var detail = new StringBuilder();
        detail.AppendLine($"{method} {url}");

        var requestBody = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value;
        if (requestBody is not null)
            detail.AppendLine($"  Request body:  {Redact(JsonSerializer.Serialize(requestBody, WireFormat))}");

        detail.AppendLine($"  Status:        {(int)response.StatusCode} {response.StatusCode}");
        detail.AppendLine($"  Response body: {Truncate(Redact(response.Content ?? string.Empty))}");

        if (response.ErrorException is not null)
            detail.AppendLine($"  Transport error: {response.ErrorException.Message}");

        _httpLog.Add(detail.ToString());
    }

    private static string Redact(string text) =>
        SensitiveFieldRegex().Replace(text, "\"$1\":\"<redacted>\"");

    private static string Truncate(string text) =>
        text.Length <= MaxLoggedBodyLength ? text : text[..MaxLoggedBodyLength] + "...<truncated>";

    [GeneratedRegex("\"(password|accessToken|refreshToken|token)\"\\s*:\\s*\"[^\"]*\"", RegexOptions.IgnoreCase)]
    private static partial Regex SensitiveFieldRegex();
}
