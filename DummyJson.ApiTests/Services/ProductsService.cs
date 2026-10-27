using DummyJson.ApiTests.Client;
using DummyJson.ApiTests.Models;
using RestSharp;

namespace DummyJson.ApiTests.Services;

public sealed class ProductsService(ApiClient client)
{
    public Task<RestResponse<ProductsResponse>> SearchAsync(string query)
    {
        var request = new RestRequest("products/search").AddQueryParameter("q", query);
        return client.ExecuteAsync<ProductsResponse>(request);
    }

    public Task<RestResponse<ProductsResponse>> GetPageAsync(int limit, int skip, string? select = null)
    {
        var request = new RestRequest("products")
            .AddQueryParameter("limit", limit.ToString())
            .AddQueryParameter("skip", skip.ToString());
        if (select is not null)
            request.AddQueryParameter("select", select);
        return client.ExecuteAsync<ProductsResponse>(request);
    }

    public Task<RestResponse<Product>> GetByIdAsync(int id) =>
        client.ExecuteAsync<Product>(new RestRequest($"products/{id}"));

    public Task<RestResponse<Product>> UpdateTitleAsync(int id, string title)
    {
        var request = new RestRequest($"products/{id}", Method.Put).AddJsonBody(new { title });
        return client.ExecuteAsync<Product>(request);
    }
}
