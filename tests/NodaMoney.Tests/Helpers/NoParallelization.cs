using Xunit;

namespace NodaMoney.Tests.Helpers
{
    [CollectionDefinition(nameof(NoParallelization), DisableParallelization = true)]
    public class NoParallelization
    {
        // Place [Collection(nameof(NoParallelization))] as attribute on a test class and it wiil become a parallel-disabled test
        // collection. Parallel-capable test collections will be run first (in parallel), followed by parallel-disabled test
        // collections (run sequentially). See https://xunit.net/docs/running-tests-in-parallel.html for more info.
    }
}
