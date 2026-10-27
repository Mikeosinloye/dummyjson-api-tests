using DummyJson.ApiTests.Config;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Services;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class AuthSteps(AuthService auth, ScenarioState state, AuthTokenStore tokens)
{
    [When("I log in with the configured user credentials")]
    public async Task WhenILogInWithTheConfiguredUserCredentials()
    {
        var settings = TestConfiguration.Api;
        var response = await auth.LoginAsync(settings.Username, settings.Password, settings.TokenExpiresInMins);
        state.LastResponse = response;
        state.LoginResult = response.Data;

        if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Data?.AccessToken))
            tokens.Store(response.Data.AccessToken);
    }

    [Given("I am authenticated with the configured user credentials")]
    public async Task GivenIAmAuthenticatedWithTheConfiguredUserCredentials()
    {
        await WhenILogInWithTheConfiguredUserCredentials();
        Assert.That(tokens.HasToken, Is.True,
            $"Login failed with status {(int)state.RequireLastResponse().StatusCode}; no access token was issued.");
    }

    [Then("the access token should be stored for subsequent requests")]
    public void ThenTheAccessTokenShouldBeStoredForSubsequentRequests()
    {
        var login = state.RequireData<LoginResponse>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(tokens.HasToken, Is.True, "No access token was stored after login.");
            Assert.That(tokens.AccessToken, Is.EqualTo(login.AccessToken),
                "Stored token differs from the token returned by the login response.");
            Assert.That(login.RefreshToken, Is.Not.Empty, "Login response should also return a refresh token.");
        }
    }

    [Then("the stored access token should be a JWT issued for the logged-in user")]
    public void ThenTheStoredAccessTokenShouldBeAJwtIssuedForTheLoggedInUser()
    {
        var login = state.RequireData<LoginResponse>();
        var claims = JwtReader.ReadPayload(tokens.AccessToken);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(claims.GetProperty("exp").GetInt64());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claims.GetProperty("id").GetInt32(), Is.EqualTo(login.Id), "Token 'id' claim");
            Assert.That(claims.GetProperty("username").GetString(), Is.EqualTo(TestConfiguration.Api.Username),
                "Token 'username' claim");
            Assert.That(expiresAt, Is.GreaterThan(DateTimeOffset.UtcNow), "Token should not already be expired");
        }
    }

    [When("I request the current authenticated user with the stored access token")]
    public async Task WhenIRequestTheCurrentAuthenticatedUserWithTheStoredAccessToken()
    {
        state.LastResponse = await auth.GetCurrentUserAsync(tokens.AccessToken);
    }

    [Then("the current authenticated user should have the following details")]
    public void ThenTheCurrentAuthenticatedUserShouldHaveTheFollowingDetails(DataTable table)
    {
        var expected = table.Rows.Single();
        var user = state.RequireData<User>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(user.Id, Is.EqualTo(int.Parse(expected["Id"])), "Id");
            Assert.That(user.FirstName, Is.EqualTo(expected["FirstName"]), "First name");
            Assert.That(user.LastName, Is.EqualTo(expected["LastName"]), "Last name");
        }
    }

    [Then("the current authenticated user should match the user returned at login")]
    public void ThenTheCurrentAuthenticatedUserShouldMatchTheUserReturnedAtLogin()
    {
        var login = state.LoginResult ?? throw new InvalidOperationException("No login result in this scenario.");
        var user = state.RequireData<User>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(user.Id, Is.EqualTo(login.Id), "Id");
            Assert.That(user.FirstName, Is.EqualTo(login.FirstName), "First name");
            Assert.That(user.LastName, Is.EqualTo(login.LastName), "Last name");
            Assert.That(user.Username, Is.EqualTo(login.Username), "Username");
        }
    }
}
