namespace NodaMoney.Tests.MoneySpec;

public class MoneyInMalagasyAriaryWhichHasFiveSubunits
{
    public static TheoryData<decimal, decimal> TestData => new()
    {
        { 0.01m, 0.0m },
        { 0.09m, 0.0m },
        { 0.10m, 0.0m },
        { 0.15m, 0.2m },
        { 0.22m, 0.2m },
        { 0.29m, 0.2m },
        { 0.30m, 0.4m },
        { 0.33m, 0.4m },
        { 0.38m, 0.4m },
        { 0.40m, 0.4m },
        { 0.41m, 0.4m },
        { 0.45m, 0.4m },
        { 0.46m, 0.4m },
        { 0.50m, 0.4m },
        { 0.54m, 0.6m },
        { 0.57m, 0.6m },
        { 0.60m, 0.6m },
        { 0.68m, 0.6m },
        { 0.70m, 0.8m },
        { 0.74m, 0.8m },
        { 0.77m, 0.8m },
        { 0.80m, 0.8m },
        { 0.83m, 0.8m },
        { 0.85m, 0.8m },
        { 0.86m, 0.8m },
        { 0.90m, 0.8m },
        { 0.91m, 1.0m },
        { 0.95m, 1.0m },
        { 0.99m, 1.0m },
    };

    [Theory, MemberData(nameof(TestData))]
    public void WhenOnlyAmount_ThenItShouldRoundUp(decimal input, decimal expected)
    {
        // 1 MGA = 5 iraimbilanja
        var money = new Money(input, "MGA");

        money.Amount.Should().Be(expected);
    }
}
