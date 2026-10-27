using DummyJson.ApiTests.Client;
using DummyJson.ApiTests.Models;
using RestSharp;

namespace DummyJson.ApiTests.Services;

public sealed class CartsService(ApiClient client)
{
    public Task<RestResponse<Cart>> AddAsync(AddCartRequest cart)
    {
        var request = new RestRequest("carts/add", Method.Post).AddJsonBody(cart);
        return client.ExecuteAsync<Cart>(request);
    }

    public Task<RestResponse<Cart>> GetByIdAsync(int id) =>
        client.ExecuteAsync<Cart>(new RestRequest($"carts/{id}"));

    public Task<RestResponse<Cart>> DeleteAsync(int id) =>
        client.ExecuteAsync<Cart>(new RestRequest($"carts/{id}", Method.Delete));
}
