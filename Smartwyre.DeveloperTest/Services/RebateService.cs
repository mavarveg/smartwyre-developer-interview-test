using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Strategies;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateStrategyFactory _strategyFactory;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IRebateStrategyFactory strategyFactory)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _strategyFactory = strategyFactory;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        var product = _productDataStore.GetProduct(request.ProductIdentifier);
        var result = new CalculateRebateResult();

        if (rebate == null)
            return result;

        var strategy = _strategyFactory.GetStrategy(rebate.Incentive);

        if (strategy == null || !strategy.IsValid(rebate, product, request))
            return result;

        var rebateAmount = strategy.Calculate(rebate, product, request);
        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        result.Success = true;

        return result;
    }
}
