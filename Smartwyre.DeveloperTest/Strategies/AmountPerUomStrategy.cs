using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class AmountPerUomStrategy : IRebateStrategy
{
    public IncentiveType IncentiveType => IncentiveType.AmountPerUom;

    public bool IsValid(Rebate rebate, Product product, CalculateRebateRequest request)
        => product != null
        && product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom)
        && rebate.Amount > 0
        && request.Volume > 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        => rebate.Amount * request.Volume;
}
