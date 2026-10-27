namespace DummyJson.ApiTests.Models;

public sealed record CartItemRequest(int Id, int Quantity);

public sealed record AddCartRequest(int UserId, IReadOnlyList<CartItemRequest> Products);

public sealed class CartProduct
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Total { get; init; }
    public decimal DiscountPercentage { get; init; }
}

public sealed class Cart
{
    public int Id { get; init; }
    public List<CartProduct> Products { get; init; } = [];
    public decimal Total { get; init; }
    public decimal DiscountedTotal { get; init; }
    public int UserId { get; init; }
    public int TotalProducts { get; init; }
    public int TotalQuantity { get; init; }

    public bool? IsDeleted { get; init; }
    public DateTimeOffset? DeletedOn { get; init; }
}
