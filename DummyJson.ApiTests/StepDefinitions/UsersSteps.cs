using System.Globalization;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Services;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class UsersSteps(UsersService users, ScenarioState state)
{
    [When("I filter users where {string} is {string}")]
    public async Task WhenIFilterUsersWhereIs(string key, string value)
    {
        state.LastResponse = await users.FilterAsync(key, value, select: "firstName,lastName,hair");
    }

    [Then("{int} users should be returned")]
    public void ThenUsersShouldBeReturned(int expectedCount)
    {
        var page = state.RequireData<UsersResponse>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Total, Is.EqualTo(expectedCount), "'total' in the response");
            Assert.That(page.Users, Has.Count.EqualTo(expectedCount), "Users actually returned");
        }
    }

    [Then("every returned user should have hair colour {string}")]
    public void ThenEveryReturnedUserShouldHaveHairColour(string colour)
    {
        var page = state.RequireData<UsersResponse>();
        var mismatches = page.Users.Where(u => u.Hair?.Color != colour).Select(u => u.Id).ToList();

        Assert.That(mismatches, Is.Empty, $"Users returned without hair colour '{colour}': {string.Join(", ", mismatches)}");
    }

    [Then("the filtered count should equal the number of users with hair colour {string} in the full user list")]
    public async Task ThenTheFilteredCountShouldEqualTheNumberOfUsersWithHairColourInTheFullUserList(string colour)
    {
        var filteredTotal = state.RequireData<UsersResponse>().Total;

        var all = await users.GetAllAsync(select: "hair");
        Assert.That(all.IsSuccessful, Is.True, $"Fetching all users failed with {(int)all.StatusCode}");

        var allUsers = all.Data!;
        Assert.That(allUsers.Users, Has.Count.EqualTo(allUsers.Total), "Full list should be returned on one page (limit=0)");

        var counted = allUsers.Users.Count(u => u.Hair?.Color == colour);
        Assert.That(filteredTotal, Is.EqualTo(counted),
            $"Filter endpoint returned {filteredTotal} but {counted} of {allUsers.Total} users have '{colour}' hair");
    }

    [Given("I have a new user with the following details")]
    public void GivenIHaveANewUserWithTheFollowingDetails(DataTable table)
    {
        var d = table.Rows.ToDictionary(r => r["Field"], r => r["Value"]);

        state.UserToCreate = new AddUserRequest(
            FirstName: d["FirstName"],
            LastName: d["LastName"],
            Age: int.Parse(d["Age"], CultureInfo.InvariantCulture),
            Gender: d["Gender"],
            Hair: new Hair { Color = d["HairColour"], Type = d["HairType"] },
            Address: new Address
            {
                Street = d["Address"],
                City = d["City"],
                State = d["State"],
                StateCode = d["StateCode"],
                PostalCode = d["PostalCode"],
                Coordinates = new Coordinates
                {
                    Lat = decimal.Parse(d["Latitude"], CultureInfo.InvariantCulture),
                    Lng = decimal.Parse(d["Longitude"], CultureInfo.InvariantCulture)
                },
                Country = d["Country"]
            });
    }

    [When("I add the new user")]
    public async Task WhenIAddTheNewUser()
    {
        var user = state.UserToCreate ?? throw new InvalidOperationException("No user has been prepared.");
        state.LastResponse = await users.AddAsync(user);
    }

    [Then("the created user should have a newly generated id")]
    public void ThenTheCreatedUserShouldHaveANewlyGeneratedId()
    {
        Assert.That(state.RequireData<User>().Id, Is.GreaterThan(0));
    }

    [Then("the created user should match the submitted details")]
    public void ThenTheCreatedUserShouldMatchTheSubmittedDetails()
    {
        var sent = state.UserToCreate!;
        var created = state.RequireData<User>();
        Assert.That(created.Hair, Is.Not.Null, "hair object missing from response");
        Assert.That(created.Address, Is.Not.Null, "address object missing from response");
        Assert.That(created.Address!.Coordinates, Is.Not.Null, "address.coordinates object missing from response");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(created.FirstName, Is.EqualTo(sent.FirstName), "firstName");
            Assert.That(created.LastName, Is.EqualTo(sent.LastName), "lastName");
            Assert.That(created.Age, Is.EqualTo(sent.Age), "age");
            Assert.That(created.Gender, Is.EqualTo(sent.Gender), "gender");
            Assert.That(created.Hair!.Color, Is.EqualTo(sent.Hair.Color), "hair.color");
            Assert.That(created.Hair.Type, Is.EqualTo(sent.Hair.Type), "hair.type");
            Assert.That(created.Address.Street, Is.EqualTo(sent.Address.Street), "address.address");
            Assert.That(created.Address.City, Is.EqualTo(sent.Address.City), "address.city");
            Assert.That(created.Address.State, Is.EqualTo(sent.Address.State), "address.state");
            Assert.That(created.Address.StateCode, Is.EqualTo(sent.Address.StateCode), "address.stateCode");
            Assert.That(created.Address.PostalCode, Is.EqualTo(sent.Address.PostalCode), "address.postalCode");
            Assert.That(created.Address.Coordinates!.Lat, Is.EqualTo(sent.Address.Coordinates!.Lat), "address.coordinates.lat");
            Assert.That(created.Address.Coordinates.Lng, Is.EqualTo(sent.Address.Coordinates.Lng), "address.coordinates.lng");
            Assert.That(created.Address.Country, Is.EqualTo(sent.Address.Country), "address.country");
        }
    }
}
