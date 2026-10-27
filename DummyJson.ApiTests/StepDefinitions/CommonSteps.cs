using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class CommonSteps(ScenarioState state)
{
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        var response = state.RequireLastResponse();
        Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode),
            $"Unexpected status for {response.Request.Method.ToString().ToUpperInvariant()} {response.Request.Resource}. " +
            $"Transport error: {response.ErrorMessage ?? "none"}");
    }
}
