using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public interface IRebateStrategyFactory
{
    IRebateStrategy GetStrategy(IncentiveType incentiveType);
}
