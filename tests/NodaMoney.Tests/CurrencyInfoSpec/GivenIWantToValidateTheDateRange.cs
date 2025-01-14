using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class GivenIWantToValidateTheDateRange
{
    [Fact]
    public void WhenValidatingACurrencyThatIsAlwaysActive_ThenShouldSucceed()
    {
        var currency = CurrencyInfo.FromCode("EUR");

        currency.IntroducedOn.Should().BeNull();
        currency.ExpiredOn.Should().BeNull();

        currency.IsActiveOn(DateTime.Today).Should().BeTrue();
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenValidatingACurrencyThatIsActiveUntilACertainDate_ThenShouldBeActiveStrictlyBeforeThatDate()
    {
        var currency = CurrencyInfo.FromCode("VEB");

        currency.IntroducedOn.Should().BeNull();
        currency.ExpiredOn.Should().Be(new DateTime(2008, 1, 1));

        currency.IsActiveOn(DateTime.MinValue).Should().BeTrue();
        currency.IsActiveOn(DateTime.MaxValue).Should().BeFalse();
        currency.IsActiveOn(new DateTime(2007, 12, 31)).Should().BeTrue();
        // assumes that the until date given in the wikipedia article is excluding.
        // assumption based on the fact that some dates are the first of the month/year
        // and that the euro started at 1999-01-01. Given that the until date of e.g. the Dutch guilder
        // is 1999-01-01, the until date must be excluding
        currency.IsActiveOn(new DateTime(2008, 1, 1)).Should().BeTrue("the until date is excluding");
    }

    [Fact]
    public void WhenValidatingACurrencyThatIsActiveFromACertainDate_ThenShouldBeActiveFromThatDate()
    {
        var currency = CurrencyInfo.FromCode("VES");

        currency.IntroducedOn.Should().Be(new DateTime(2018, 8, 20));
        currency.ExpiredOn.Should().BeNull();

        currency.IsActiveOn(DateTime.MinValue).Should().BeFalse();
        currency.IsActiveOn(DateTime.MaxValue).Should().BeTrue();
        currency.IsActiveOn(new DateTime(2018, 8, 19)).Should().BeFalse();
        currency.IsActiveOn(new DateTime(2018, 8, 20)).Should().BeTrue();
    }
}
