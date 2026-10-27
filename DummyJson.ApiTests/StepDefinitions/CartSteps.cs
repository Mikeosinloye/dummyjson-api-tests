using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Services;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class CartSteps(CartsService carts, ProductsService products, ScenarioState state, IReqnrollOutputHelper output)
{
    [When("I add the following products to a new cart for user {int}")]
    public async Task WhenIAddTheFollowingProductsToANewCartForUser(int userId, DataTable table)
    {
        var items = table.Rows
            .Select(r => new CartItemRequest(int.Parse(r["ProductId"]), int.Parse(r["Quantity"])))
            .ToList();

        state.CartRequest = new AddCartRequest(userId, items);
        state.LastResponse = await carts.AddAsync(state.CartRequest);
    }

    [Then("the cart should contain {int} products with a total quantity of {int}")]
    public void ThenTheCartShouldContainProductsWithATotalQuantityOf(int productCount, int totalQuantity)
    {
        var cart = state.RequireData<Cart>();
        var requested = state.CartRequest!.Products.Select(p => (p.Id, p.Quantity));
        var returned = cart.Products.Select(p => (p.Id, p.Quantity));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cart.UserId, Is.EqualTo(state.CartRequest.UserId), "userId");
            Assert.That(cart.Products, Has.Count.EqualTo(productCount), "products returned");
            Assert.That(cart.TotalProducts, Is.EqualTo(productCount), "totalProducts");
            Assert.That(cart.TotalQuantity, Is.EqualTo(totalQuantity), "totalQuantity");
            Assert.That(returned, Is.EquivalentTo(requested), "product ids/quantities should match the request");
        }
    }

    [Then("each cart product's discount percentage should match its product catalogue entry")]
    public async Task ThenEachCartProductsDiscountPercentageShouldMatchItsProductCatalogueEntry()
    {
        var cart = state.RequireData<Cart>();

        foreach (var line in cart.Products)
        {
            var catalogue = await products.GetByIdAsync(line.Id);
            Assert.That(catalogue.IsSuccessful, Is.True, $"Product {line.Id} could not be fetched");
            Assert.That(line.DiscountPercentage, Is.EqualTo(catalogue.Data!.DiscountPercentage),
                $"discountPercentage for product {line.Id} ({line.Title})");
        }
    }

    [Then(@"^the total discount percentage of the cart should be (\d+(?:\.\d+)?)$")]
    public void ThenTheTotalDiscountPercentageOfTheCartShouldBe(decimal expectedTotal)
    {
        var cart = state.RequireData<Cart>();
        var total = cart.Products.Sum(p => p.DiscountPercentage);

        output.WriteLine($"Total discount percentage = {string.Join(" + ", cart.Products.Select(p => p.DiscountPercentage))} = {total}");

        Assert.That(total, Is.EqualTo(expectedTotal));
    }

    [Given("cart {int} exists")]
    public async Task GivenCartExists(int id)
    {
        var response = await carts.GetByIdAsync(id);
        Assert.That(response.IsSuccessful, Is.True, $"Cart {id} could not be fetched: {(int)response.StatusCode}");
    }

    [When("I delete cart {int}")]
    public async Task WhenIDeleteCart(int id)
    {
        state.LastResponse = await carts.DeleteAsync(id);
    }

    [Then("the response should show cart {int} flagged as deleted with a deletion timestamp")]
    public void ThenTheResponseShouldShowCartFlaggedAsDeletedWithADeletionTimestamp(int id)
    {
        var cart = state.RequireData<Cart>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cart.Id, Is.EqualTo(id), "id");
            Assert.That(cart.IsDeleted, Is.True, "isDeleted");
            Assert.That(cart.DeletedOn, Is.Not.Null, "deletedOn");
            Assert.That(cart.DeletedOn, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromHours(1)), "deletedOn should be current");
        }
    }
}
