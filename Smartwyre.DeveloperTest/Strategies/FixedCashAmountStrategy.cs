using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class FixedCashAmountStrategy : IRebateStrategy
{
    public IncentiveType IncentiveType => IncentiveType.FixedCashAmount;

    public bool IsValid(Rebate rebate, Product product, CalculateRebateRequest request)
        => product != null
        && product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount)
        && rebate.Amount > 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        => rebate.Amount;
}
