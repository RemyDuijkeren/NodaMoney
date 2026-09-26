using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyContextSpec;

[Collection(nameof(NoParallelization))]
public class ResolveContext
{
    [Fact]
    public void CreateContext_WhenOptionsAreNew_ShouldResolveBackToSameInstanceByIndex()
    {
        // Arrange
        var options = new MoneyContextOptions { Precision = 11, MaxScale = 3 };

        // Act
        var context = MoneyContext.Create(options);
        var resolved = MoneyContext.Get(context.Index);

        // Assert
        resolved.Should().BeSameAs(context);
    }

    [Fact]
    public void CreateContext_WhenOptionsAreEquivalentToExisting_ShouldReturnExistingInstance()
    {
        // Arrange
        var first = MoneyContext.Create(new MoneyContextOptions { Precision = 13, MaxScale = 2 });

        // Act
        var second = MoneyContext.Create(new MoneyContextOptions { Precision = 13, MaxScale = 2 });

        // Assert
        second.Should().BeSameAs(first);
        second.Index.Should().Be(first.Index);
    }

    [Fact]
    public void Get_WhenIndexWasNeverRegistered_ShouldThrowArgumentException()
    {
        // Arrange
        const byte unregisteredIndex = 127; // last of the 128 slots; nothing registers this many contexts in the suite

        // Act
        Action act = () => MoneyContext.Get(unregisteredIndex);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage($"Invalid MoneyContext index: {unregisteredIndex}");
    }

    [Fact]
    public void ThreadContext_WhenSetInsideScope_ShouldBeCurrentContext_AndRestoredAfterScope()
    {
        // Arrange
        var context = MoneyContext.Create(options =>
        {
            options.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero);
            options.MaxScale = 9;
        });
        var beforeScope = MoneyContext.CurrentContext;

        // Act & Assert
        using (MoneyContext.CreateScope(context))
        {
            MoneyContext.ThreadContext.Should().Be(context);
            MoneyContext.CurrentContext.Should().Be(context);
        }

        MoneyContext.ThreadContext.Should().BeNull();
        MoneyContext.CurrentContext.Should().Be(beforeScope);
    }

    [Fact]
    public void ThreadContext_WhenSetToNull_ShouldFallBackToDefaultThreadContext()
    {
        // Arrange
        var context = MoneyContext.Create(options => options.MaxScale = 10);
        MoneyContext.ThreadContext = context;

        // Act
        MoneyContext.ThreadContext = null;

        // Assert
        MoneyContext.ThreadContext.Should().BeNull();
        MoneyContext.CurrentContext.Should().Be(MoneyContext.DefaultThreadContext);
    }

    [Fact]
    public async Task CurrentContext_WhenObservedAfterAwaitInsideScope_ShouldBeScopedContext()
    {
        // Arrange
        var context = MoneyContext.Create(options => options.MaxScale = 11);

        // Act & Assert
        using (MoneyContext.CreateScope(context))
        {
            await Task.Delay(1);
            MoneyContext.CurrentContext.Should().Be(context);
        }
    }
}
