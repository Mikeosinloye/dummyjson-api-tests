@products
Feature: Products
  As an API consumer
  I want to search, page through and update products
  So that I can confirm the Products resource returns and accepts the right data

  Scenario: Verify and validate the price of iPhone 13 Pro
    When I search for products matching "iPhone 13 Pro"
    Then the response status code should be 200
    And the search results should contain exactly one product titled "iPhone 13 Pro"
    And the product titled "iPhone 13 Pro" should have id 123 and price 1099.99

  Scenario: Retrieve the 15th product using pagination
    When I request product number 15 using pagination, selecting only "id,title,price"
    Then the response status code should be 200
    And the paged response should contain only 1 product with limit 1 and skip 14
    And the paged product should only contain the fields "id,title,price"
    And the paged product should have id 15, title "Wooden Bathroom Sink With Mirror" and price 799.99

  Scenario: Update the title of product 121 and validate the updated title
    Given product 121 exists
    When I update the title of product 121 to "iPhone 5s (Refurbished) - STA Challenge"
    Then the response status code should be 200
    And the response should show product 121 with title "iPhone 5s (Refurbished) - STA Challenge"
    And the other attributes of product 121 should be unchanged in the response
