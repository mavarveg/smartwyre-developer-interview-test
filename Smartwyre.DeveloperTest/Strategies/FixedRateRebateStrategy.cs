using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedRateRebateStrategy : IRebateStrategy
{
    public IncentiveType IncentiveType => IncentiveType.FixedRateRebate;

    public bool IsValid(Rebate rebate, Product product, CalculateRebateRequest request)
        => product != null
        && product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate)
        && rebate.Percentage > 0
        && product.Price > 0
        && request.Volume > 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        => product.Price * rebate.Percentage * request.Volume;
}
