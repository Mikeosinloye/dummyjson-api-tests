namespace DummyJson.ApiTests.Models;

public sealed class Product
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal Rating { get; init; }
    public int Stock { get; init; }
    public string? Brand { get; init; }
}

public sealed class ProductsResponse
{
    public List<Product> Products { get; init; } = [];
    public int Total { get; init; }
    public int Skip { get; init; }
    public int Limit { get; init; }
}
