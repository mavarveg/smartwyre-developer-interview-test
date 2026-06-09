using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _rebateDataStore = new();
    private readonly Mock<IProductDataStore> _productDataStore = new();

    private RebateService BuildService(IRebateStrategy strategy)
    {
        var factory = new RebateStrategyFactory(new[] { strategy });
        return new RebateService(_rebateDataStore.Object, _productDataStore.Object, factory);
    }

    private RebateService BuildServiceWithFactory(IRebateStrategyFactory factory)
        => new RebateService(_rebateDataStore.Object, _productDataStore.Object, factory);

    // ── FixedCashAmount ──────────────────────────────────────────────────────

    [Fact]
    public void FixedCashAmount_ValidRebateAndProduct_ReturnsSuccess()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 100m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new FixedCashAmountStrategy()).Calculate(new CalculateRebateRequest());

        Assert.True(result.Success);
        _rebateDataStore.Verify(d => d.StoreCalculationResult(rebate, 100m), Times.Once);
    }

    [Fact]
    public void FixedCashAmount_RebateIsNull_ReturnsFailure()
    {
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns((Rebate)null);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(new Product());

        var result = BuildService(new FixedCashAmountStrategy()).Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
    }

    [Fact]
    public void FixedCashAmount_ProductDoesNotSupportIncentive_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 100m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new FixedCashAmountStrategy()).Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
    }

    [Fact]
    public void FixedCashAmount_AmountIsZero_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 0m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new FixedCashAmountStrategy()).Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
    }

    // ── FixedRateRebate ──────────────────────────────────────────────────────

    [Fact]
    public void FixedRateRebate_ValidRebateAndProduct_ReturnsSuccess()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 200m };
        var request = new CalculateRebateRequest { Volume = 5m };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new FixedRateRebateStrategy()).Calculate(request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(d => d.StoreCalculationResult(rebate, 100m), Times.Once);
    }

    [Fact]
    public void FixedRateRebate_ProductIsNull_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns((Product)null);

        var result = BuildService(new FixedRateRebateStrategy()).Calculate(new CalculateRebateRequest { Volume = 1m });

        Assert.False(result.Success);
    }

    [Fact]
    public void FixedRateRebate_PercentageIsZero_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 200m };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new FixedRateRebateStrategy()).Calculate(new CalculateRebateRequest { Volume = 5m });

        Assert.False(result.Success);
    }

    // ── AmountPerUom ─────────────────────────────────────────────────────────

    [Fact]
    public void AmountPerUom_ValidRebateAndProduct_ReturnsSuccess()
    {
        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 50m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        var request = new CalculateRebateRequest { Volume = 3m };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new AmountPerUomStrategy()).Calculate(request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(d => d.StoreCalculationResult(rebate, 150m), Times.Once);
    }

    [Fact]
    public void AmountPerUom_VolumeIsZero_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 50m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(product);

        var result = BuildService(new AmountPerUomStrategy()).Calculate(new CalculateRebateRequest { Volume = 0m });

        Assert.False(result.Success);
    }

    // ── Factory ──────────────────────────────────────────────────────────────

    [Fact]
    public void Calculate_NoMatchingStrategy_ReturnsFailure()
    {
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _rebateDataStore.Setup(d => d.GetRebate(It.IsAny<string>())).Returns(rebate);
        _productDataStore.Setup(d => d.GetProduct(It.IsAny<string>())).Returns(new Product());

        var emptyFactory = new Mock<IRebateStrategyFactory>();
        emptyFactory.Setup(f => f.GetStrategy(It.IsAny<IncentiveType>())).Returns((IRebateStrategy)null);

        var result = BuildServiceWithFactory(emptyFactory.Object).Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
    }
}
