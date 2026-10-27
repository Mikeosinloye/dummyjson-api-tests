@cart
Feature: Cart
  As an API consumer
  I want to create and delete carts
  So that I can confirm the Cart resource calculates and removes carts correctly

  Scenario: Add 5 products to a cart and validate the total discount percentage
    When I add the following products to a new cart for user 1
      | ProductId | Quantity |
      | 1         | 1        |
      | 2         | 2        |
      | 3         | 1        |
      | 4         | 1        |
      | 5         | 3        |
    Then the response status code should be 201
    And the cart should contain 5 products with a total quantity of 8
    And each cart product's discount percentage should match its product catalogue entry
    And the total discount percentage of the cart should be 62.11

  Scenario: Delete a cart
    Given cart 1 exists
    When I delete cart 1
    Then the response status code should be 200
    And the response should show cart 1 flagged as deleted with a deletion timestamp
