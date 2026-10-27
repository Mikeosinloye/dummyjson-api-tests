using System.Text.Json;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Services;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class ProductsSteps(ProductsService products, ScenarioState state)
{
    [When("I search for products matching {string}")]
    public async Task WhenISearchForProductsMatching(string query)
    {
        state.LastResponse = await products.SearchAsync(query);
    }

    [Then("the search results should contain exactly one product titled {string}")]
    public void ThenTheSearchResultsShouldContainExactlyOneProductTitled(string title)
    {
        var matches = state.RequireData<ProductsResponse>().Products.Where(p => p.Title == title);
        Assert.That(matches.Count(), Is.EqualTo(1), $"Products titled exactly '{title}'");
    }

    [Then(@"^the product titled ""(.*)"" should have id (\d+) and price (\d+(?:\.\d+)?)$")]
    public void ThenTheProductTitledShouldHaveIdAndPrice(string title, int expectedId, decimal expectedPrice)
    {
        var product = state.RequireData<ProductsResponse>().Products.Single(p => p.Title == title);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.EqualTo(expectedId), "id");
            Assert.That(product.Price, Is.EqualTo(expectedPrice), "price");
        }
    }

    [When("I request product number {int} using pagination, selecting only {string}")]
    public async Task WhenIRequestProductNumberUsingPaginationSelectingOnly(int position, string fields)
    {
        state.LastResponse = await products.GetPageAsync(limit: 1, skip: position - 1, select: fields);
    }

    [Then("the paged response should contain only {int} product with limit {int} and skip {int}")]
    public void ThenThePagedResponseShouldContainOnlyProductWithLimitAndSkip(int count, int limit, int skip)
    {
        var page = state.RequireData<ProductsResponse>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Products, Has.Count.EqualTo(count), "products returned");
            Assert.That(page.Limit, Is.EqualTo(limit), "limit");
            Assert.That(page.Skip, Is.EqualTo(skip), "skip");
        }
    }

    [Then("the paged product should only contain the fields {string}")]
    public void ThenThePagedProductShouldOnlyContainTheFields(string fields)
    {
        using var json = JsonDocument.Parse(state.RequireLastResponse().Content!);
        var product = json.RootElement.GetProperty("products").EnumerateArray().Single();
        var actual = product.EnumerateObject().Select(p => p.Name).Order();
        var expected = fields.Split(',', StringSplitOptions.TrimEntries).Order();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Then(@"^the paged product should have id (\d+), title ""(.*)"" and price (\d+(?:\.\d+)?)$")]
    public void ThenThePagedProductShouldHaveIdTitleAndPrice(int expectedId, string expectedTitle, decimal expectedPrice)
    {
        var product = state.RequireData<ProductsResponse>().Products.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.EqualTo(expectedId), "id");
            Assert.That(product.Title, Is.EqualTo(expectedTitle), "title");
            Assert.That(product.Price, Is.EqualTo(expectedPrice), "price");
        }
    }

    [Given("product {int} exists")]
    public async Task GivenProductExists(int id)
    {
        var response = await products.GetByIdAsync(id);
        Assert.That(response.IsSuccessful, Is.True, $"Product {id} could not be fetched: {(int)response.StatusCode}");
        state.OriginalProduct = response.Data;
    }

    [When("I update the title of product {int} to {string}")]
    public async Task WhenIUpdateTheTitleOfProductTo(int id, string title)
    {
        state.LastResponse = await products.UpdateTitleAsync(id, title);
    }

    [Then("the response should show product {int} with title {string}")]
    public void ThenTheResponseShouldShowProductWithTitle(int id, string title)
    {
        var updated = state.RequireData<Product>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updated.Id, Is.EqualTo(id), "id");
            Assert.That(updated.Title, Is.EqualTo(title), "title");
            Assert.That(updated.Title, Is.Not.EqualTo(state.OriginalProduct!.Title), "title should differ from the original");
        }
    }

    [Then("the other attributes of product {int} should be unchanged in the response")]
    public void ThenTheOtherAttributesOfProductShouldBeUnchangedInTheResponse(int id)
    {
        var original = state.OriginalProduct!;
        var updated = state.RequireData<Product>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updated.Id, Is.EqualTo(id), "id");
            Assert.That(updated.Description, Is.EqualTo(original.Description), "description");
            Assert.That(updated.Category, Is.EqualTo(original.Category), "category");
            Assert.That(updated.Price, Is.EqualTo(original.Price), "price");
            Assert.That(updated.DiscountPercentage, Is.EqualTo(original.DiscountPercentage), "discountPercentage");
            Assert.That(updated.Stock, Is.EqualTo(original.Stock), "stock");
            Assert.That(updated.Brand, Is.EqualTo(original.Brand), "brand");
        }
    }
}
