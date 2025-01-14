using Xunit;

namespace NodaMoney.Tests.Serialization;

public class NestedJsonV1TestData : TheoryData<string, Order>
{
    public NestedJsonV1TestData()
    {
        var order = new Order
        {
            Id = 123,
            Name = "Abc",
            Total = new Money(234.25m, Currency.FromCode("EUR"))
        };

        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Amount": 234.25, "Currency": "EUR" } }""", order); // Amount as number
        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Amount": "234.25", "Currency": "EUR" } }""", order); // Amount as string

        // Reversed members
        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Currency": "EUR", "Amount": 234.25 } }""", order); // Amount as number
        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Currency": "EUR", "Amount": "234.25" } }""", order); // Amount as string

        // camelCase
        Add("""{ "id": 123, "name": "Abc", "total": { "amount": 234.25, "currency": "EUR" } }""", order); // Amount as number
        Add("""{ "id": 123, "name": "Abc", "total": { "amount": "234.25", "currency": "EUR" } }""", order); // Amount as string

        // Discount explicit null
        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Amount": 234.25, "Currency": "EUR" }, "Discount": null }""", order); // Amount as number
        Add("""{ "Id": 123, "Name": "Abc", "Total": { "Amount": "234.25", "Currency": "EUR" }, "Discount": null }""", order); // Amount as string
    }
}
