using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Strategies;

public class RebateStrategyFactory : IRebateStrategyFactory
{
    private readonly Dictionary<IncentiveType, IRebateStrategy> _strategies;

    public RebateStrategyFactory(IEnumerable<IRebateStrategy> strategies)
    {
        _strategies = new Dictionary<IncentiveType, IRebateStrategy>();
        foreach (var strategy in strategies)
            _strategies[strategy.IncentiveType] = strategy;
    }

    public IRebateStrategy GetStrategy(IncentiveType incentiveType)
        => _strategies.TryGetValue(incentiveType, out var strategy) ? strategy : null;
}
