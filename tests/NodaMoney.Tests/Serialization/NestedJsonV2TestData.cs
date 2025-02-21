namespace NodaMoney.Tests.Serialization;

public class NestedJsonV2TestData : TheoryData<string, Order>
{
    public NestedJsonV2TestData()
    {
        var order = new Order
        {
            Id = 123,
            Name = "Abc",
            Total = new Money(234.25m, CurrencyInfo.FromCode("EUR"))
        };

        Add("""{ "Id": 123, "Name": "Abc", "Total": "EUR 234.25" }""", order);
        Add("""{ "id": 123, "name": "Abc", "total": "EUR 234.25" }""", order); // camelCase (System.Text.Json needs PropertyNameCaseInsensitive = true to work)

        // // Discount explicit null
        // Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": 234.25, \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as number
        // Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": \"234.25\", \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as string
    }
}
