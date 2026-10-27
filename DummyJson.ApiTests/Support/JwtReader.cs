using System.Text;
using System.Text.Json;

namespace DummyJson.ApiTests.Support;

public static class JwtReader
{
    public static JsonElement ReadPayload(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length != 3)
            throw new FormatException($"Expected a JWT with 3 segments but found {parts.Length}.");

        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        return JsonDocument.Parse(json).RootElement.Clone();
    }
}
