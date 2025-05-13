using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyContextSpec;

[Collection(nameof(NoParallelization))]
public class CreateContext
{
    [Fact]
    public void CreateWithStandardRounding_ShouldSucceed()
    {
        // Arrange

        // Act
        var context = MoneyContext.Create(new StandardRounding());

        // Assert
        context.Should().NotBeNull();
        context.RoundingStrategy.Should().BeOfType<StandardRounding>().Subject.Mode.Should().Be(MidpointRounding.ToEven);
        context.MaxScale.Should().BeNull();
        context.Precision.Should().Be(28);
    }

    [Fact]
    public void SetGlobalContext()
    {
        // Arrange
        var context = MoneyContext.Create(new StandardRounding(MidpointRounding.AwayFromZero), maxScale: 4);

        // Act
        MoneyContext.DefaultThreadContext = context;
        var money = new Money(1234.56789m, "EUR");

        // Assert
        MoneyContext.DefaultThreadContext.Should().Be(context);
        money.Context.Should().Be(context);
        money.Scale.Should().Be(4);
        money.Amount.Should().Be(1234.5679m);
    }

    [Fact]
    public void SetThreadContext()
    {
        // Arrange
        var amount = 1234.56789m;
        var context = MoneyContext.Create(new StandardRounding(MidpointRounding.AwayFromZero), maxScale: 4);

        // Act
        MoneyContext.ThreadContext = context;
        var money = new Money(amount, "EUR");

        // Assert
        MoneyContext.DefaultThreadContext.Should().NotBe(context);
        MoneyContext.ThreadContext.Should().Be(context);
        money.Context.Should().Be(context);
        money.Scale.Should().Be(4, because: "MaxScale set to 4 in MoneyContext");
        money.Amount.Should().Be(1234.5679m, because: "Rounding to 4 decimals");
    }

    [Fact]
    public void PassContextToMoney()
    {
        // Arrange
        var context = MoneyContext.Create(new StandardRounding(MidpointRounding.AwayFromZero), maxScale: 4);

        // Act
        var money = new Money(1234.56789m, "EUR", context);

        // Assert
        MoneyContext.DefaultThreadContext.Should().NotBe(context);
        money.Context.Should().Be(context);
        money.Scale.Should().Be(4);
        money.Amount.Should().Be(1234.5679m);
    }

    [Fact]
    public void UseScopeWithGivenContext()
    {
        // Arrange
        var amount = 1234.56789m;
        var context = MoneyContext.Create(new StandardRounding(MidpointRounding.AwayFromZero), maxScale: 4);

        // Act
        Money money;
        using (MoneyContext.CreateScope(context))
        {
            MoneyContext.DefaultThreadContext.Should().NotBe(context);
            MoneyContext.ThreadContext.Should().Be(context, because: "Context is set before scope");
            money = new Money(amount, "EUR");
        }

        // Assert
        MoneyContext.DefaultThreadContext.Should().NotBe(context);
        MoneyContext.ThreadContext.Should().NotBe(context, because: "Context is reset after scope");
        money.Context.Should().Be(context);
        money.Scale.Should().Be(4, because: "MaxScale set to 4 in MoneyContext");
        money.Amount.Should().Be(1234.5679m, because: "Rounding to 4 decimals");
    }

    [Fact]
    public void UseScopeWithAutoCreateContext()
    {
        // Arrange
        var amount = 1234.56789m;

        // Act
        Money money;
        using (MoneyContext.CreateScope(new StandardRounding(MidpointRounding.AwayFromZero), maxScale: 4))
        {
            money = new Money(amount, "EUR");
        }

        // Assert
        money.Context.RoundingStrategy.Should().BeOfType<StandardRounding>().Subject.Mode.Should().Be(MidpointRounding.AwayFromZero);
        money.Context.MaxScale.Should().Be(4);
        money.Scale.Should().Be(4, because: "MaxScale set to 4 in MoneyContext");
        money.Amount.Should().Be(1234.5679m, because: "Rounding to 4 decimals");
    }
}
