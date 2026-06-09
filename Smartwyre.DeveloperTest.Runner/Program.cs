using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;

var strategies = new IRebateStrategy[]
{
    new FixedCashAmountStrategy(),
    new FixedRateRebateStrategy(),
    new AmountPerUomStrategy()
};

var factory = new RebateStrategyFactory(strategies);
var rebateDataStore = new RebateDataStore();
var productDataStore = new ProductDataStore();
var service = new RebateService(rebateDataStore, productDataStore, factory);

Console.Write("Rebate Identifier: ");
var rebateId = Console.ReadLine();

Console.Write("Product Identifier: ");
var productId = Console.ReadLine();

Console.Write("Volume: ");
var volume = decimal.Parse(Console.ReadLine() ?? "0");

var request = new CalculateRebateRequest
{
    RebateIdentifier = rebateId,
    ProductIdentifier = productId,
    Volume = volume
};

var result = service.Calculate(request);

Console.WriteLine(result.Success ? "Result: Success" : "Result: Failure");
