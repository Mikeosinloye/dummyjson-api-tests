@auth
Feature: Auth
  As an API consumer
  I want to authenticate against DummyJSON
  So that I can call endpoints that need a bearer access token

  Scenario: Authenticate a user and store the access token
    When I log in with the configured user credentials
    Then the response status code should be 200
    And the access token should be stored for subsequent requests
    And the stored access token should be a JWT issued for the logged-in user

  Scenario: Validate the Id, first name and last name of the current authenticated user
    Given I am authenticated with the configured user credentials
    When I request the current authenticated user with the stored access token
    Then the response status code should be 200
    And the current authenticated user should have the following details
      | Id | FirstName | LastName |
      | 1  | Emily     | Johnson  |
    And the current authenticated user should match the user returned at login
