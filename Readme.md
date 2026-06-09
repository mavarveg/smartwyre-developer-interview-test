# Smartwyre Developer Test Instructions

You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

---

## Solution Notes

### Design approach

The original `RebateService` violated several SOLID principles: it instantiated data stores directly (DIP), had a monolithic switch that needed to grow with every new incentive type (OCP/SRP), and contained a null-check bug where `rebate == null` was tested *after* `rebate.Incentive` was already accessed.

The refactor introduces three patterns:

**1. Interfaces for data stores (Dependency Inversion)**  
`IRebateDataStore` and `IProductDataStore` allow the service and tests to depend on abstractions, not concrete implementations.

**2. Strategy pattern (Open/Closed + Single Responsibility)**  
Each incentive type gets its own class implementing `IRebateStrategy`, which owns two responsibilities: `IsValid()` to check preconditions and `Calculate()` to compute the rebate amount.

**3. Factory (Single Responsibility)**  
`RebateStrategyFactory` resolves the correct strategy by `IncentiveType` using a dictionary. `RebateService` no longer needs to know which strategies exist.

### Adding a new incentive type

1. Create a new class implementing `IRebateStrategy`.
2. Register it when constructing `RebateStrategyFactory` (pass it in the strategies collection).
3. No changes needed in `RebateService` or any existing strategy.

### Running the console app

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner
```

The app will prompt for `Rebate Identifier`, `Product Identifier` and `Volume`, then print `Result: Success` or `Result: Failure`.

### Running the tests

```bash
dotnet test Smartwyre.DeveloperTest.Tests
```

10 unit tests covering happy paths and failure cases for each incentive type, null inputs, and missing strategy scenarios. Uses xUnit and Moq.
