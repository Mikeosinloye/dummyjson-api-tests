@users
Feature: Users
  As an API consumer
  I want to query and create users
  So that I can confirm the Users resource filters and accepts data correctly

  Scenario: Verify and validate how many users have red hair
    When I filter users where "hair.color" is "Red"
    Then the response status code should be 200
    And 25 users should be returned
    And every returned user should have hair colour "Red"
    And the filtered count should equal the number of users with hair colour "Red" in the full user list

  Scenario: Add a new user with personal, hair and full address details
    Given I have a new user with the following details
      | Field       | Value                 |
      | FirstName   | Ada                   |
      | LastName    | Lovelace              |
      | Age         | 36                    |
      | Gender      | female                |
      | HairColour  | Brown                 |
      | HairType    | Wavy                  |
      | Address     | 12 St James's Square  |
      | City        | London                |
      | State       | Greater London        |
      | StateCode   | LND                   |
      | PostalCode  | SW1Y 4LB              |
      | Latitude    | 51.5074               |
      | Longitude   | -0.1350               |
      | Country     | United Kingdom        |
    When I add the new user
    Then the response status code should be 201
    And the created user should have a newly generated id
    And the created user should match the submitted details
