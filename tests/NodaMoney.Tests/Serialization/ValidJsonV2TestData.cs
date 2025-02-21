namespace NodaMoney.Tests.Serialization;

public class ValidJsonV2TestData : TheoryData<string, Money>
{
    public ValidJsonV2TestData()
    {
        Add("\"EUR 234.25\"", new Money(234.25m, CurrencyInfo.FromCode("EUR")));
        Add("\"BTC 234.25\"", new Money(234.25m, CurrencyInfo.FromCode("BTC")));
        //Add("\"EUR;CUSTOM 234.25\"", new Money(234.25m, CurrencyInfo.FromCode("EUR")));
        //Add("\"EUR;ISO-4217 234.25\"", new Money(234.25m, CurrencyInfo.FromCode("EUR")));
        //Add("\"EUR; 234.25\"", new Money(234.25m, CurrencyInfo.FromCode("EUR")));

        // member reversed
        Add("\"234.25 EUR\"", new Money(234.25m, CurrencyInfo.FromCode("EUR")));
        Add("\"234.25 BTC\"", new Money(234.25m, CurrencyInfo.FromCode("BTC")));
    }
}
