using System;
using System.Text.Json.Serialization;
using Xunit;

namespace NodaMoney.Tests.Serialization;

// old serialization format (v1): { "Amount": 234.25, "Currency": "EUR" }
// new serialization format (v2): "EUR 234.25"

public class ValidJsonV1TestData : TheoryData<string, Money>
{
    public ValidJsonV1TestData()
    {
        var money = new Money(234.25m, Currency.FromCode("EUR"));

        Add("""{ "Amount": 234.25, "Currency": "EUR" }""", money); // PascalCase, Amount as number
        Add("""{ "Currency": "EUR", "Amount": 234.25 }""", money); // PascalCase, Amount as number, Reversed members
        Add("""{ "Amount": "234.25", "Currency": "EUR" }""", money); // PascalCase, Amount as string
        Add("""{ "Currency": "EUR", "Amount": "234.25" }""", money); // PascalCase, Amount as string, Reversed members
        Add("""{ "amount": 234.25, "currency": "EUR" }""", money); // camelCase, Amount as num"ber
        Add("""{ "currency": "EUR", "amount": 234.25 }""", money); // camelCase, Amount as number, Reversed members
        Add("""{ "amount": "234.25", "currency": "EUR" }""", money); // camelCase, Amount as string
        Add("""{ "currency": "EUR", "amount": "234.25" }""", money); // camelCase, Amount as string, Reversed members

        // // Members no quotation marks (NOT valid json)
        // Add("{ Amount: 234.25, Currency: \"EUR\" }", money); // PascalCase, Amount as number
        // Add("{ Currency: \"EUR\", Amount: 234.25 }", money); // PascalCase, Amount as number, Reversed members
        // Add("{ Amount: \"234.25\", Currency: \"EUR\" }", money); // PascalCase, Amount as string
        // Add("{ Currency: \"EUR\", Amount: \"234.25\" }", money); // PascalCase, Amount as string
        // Add("{ amount: 234.25, currency: \"EUR\" }", money); // camelCase, Amount as number
        // Add("{ currency: \"EUR\", amount: 234.25 }", money); // camelCase, Amount as number, Reversed members
        // Add("{ amount: \"234.25\", currency: \"EUR\" }", money); // camelCase, Amount as string, Members no quotation marks
        // Add("{ currency: \"EUR\", amount: \"234.25\" }", money); // camelCase, Amount as string, Reversed members

        // // Members no quotation marks, Values single quotes (NOT valid json)
        // Add("{ Amount: 234.25, Currency: 'EUR' }", money); // PascalCase, Amount as number,
        // Add("{ Currency: 'EUR', Amount: 234.25 }", money); // PascalCase, Amount as number, Reversed members
        // Add("{ Amount: '234.25', Currency: 'EUR' }", money); // PascalCase, Amount as string
        // Add("{ Currency: 'EUR', Amount: '234.25' }", money); // PascalCase, Amount as string, Reversed members
        // Add("{ amount: 234.25, currency: 'EUR' }", money); // camelCase, Amount as number
        // Add("{ currency: 'EUR', amount: 234.25 }", money); // camelCase, Amount as number, Reversed members
        // Add("{ amount: '234.25', currency: 'EUR' }", money); // camelCase, Amount as string
        // Add("{ currency: 'EUR', amount: '234.25' }", money); // camelCase, Amount as string, Reversed members

        // Currency with namespace
        Add("""{ "Amount": 234.25, "Currency": "EUR;ISO-4217" }""", money); // PascalCase, Amount as number
        Add("""{ "Currency": "EUR;ISO-4217", "Amount": 234.25 }""", money); // PascalCase, Amount as number, Reversed members
        Add("""{ "Amount": "234.25", "Currency": "EUR;ISO-4217" }""", money); // PascalCase, Amount as string
        Add("""{ "Currency": "EUR;ISO-4217", "Amount": "234.25" }""", money); // PascalCase, Amount as string, Reversed members
        Add("""{ "amount": 234.25, "currency": "EUR;ISO-4217" }""", money); // camelCase, Amount as number
        Add("""{ "currency": "EUR;ISO-4217", "amount": 234.25 }""", money); // camelCase, Amount as number, Reversed members
        Add("""{ "amount": "234.25", "currency": "EUR;ISO-4217" }""", money); // camelCase, Amount as string
        Add("""{ "currency": "EUR;ISO-4217", "amount": "234.25" }""", money); // camelCase, Amount as string, Reversed members
        Add("""{ "Amount": 234.25, "Currency": "EUR;" }""", money); // PascalCase, Amount as number, No Namespace but has separator
    }
}

public class InvalidJsonV1TestData : TheoryData<string>
{
    public InvalidJsonV1TestData()
    {
        Add("""{ "Amount": 234.25 }"""); // PascalCase, Amount as number, No Currency member
        Add("""{ "Currency": "EUR" }"""); // PascalCase, No Amount member
        Add("""{ "Amount": "234.25" }"""); // PascalCase, Amount as string, No Currency member
        Add("""{ "amount": 234.25 }"""); // camelCase, Amount as number, No Currency member
        Add("""{ "currency": "EUR" }"""); // camelCase, No Amount member

        // Members no quotation marks
        Add("""{ Amount: 234.25 }"""); // PascalCase, Amount as number, No Currency member
        Add("""{ Currency: "EUR" }"""); // PascalCase, No Amount member
        Add("""{ Amount: "234.25" }"""); // PascalCase, Amount as string, No Currency member
        Add("""{ amount: 234.25 }"""); // camelCase, Amount as number, No Currency member
        Add("""{ currency: "EUR" }"""); // camelCase, No Amount member

        // Members no quotation marks, Values single quotes
        Add("""{ Currency: 'EUR' }"""); // PascalCase, No Amount member,
        Add("""{ Amount: '234.25' }"""); // PascalCase, Amount as string, No Currency member
        Add("""{ currency: 'EUR' }"""); // camelCase, No Amount member

        Add("""{ "Amount": "ABC", "Currency": "EUR" }"""); // => format exception without telling which member
    }
}

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

public class ValidJsonV2TestData : TheoryData<string, Money>
{
    public ValidJsonV2TestData()
    {
        Add("\"EUR 234.25\"", new Money(234.25m, Currency.FromCode("EUR")));
        Add("\"BTC 234.25\"", new Money(234.25m, Currency.FromCode("BTC")));
        //Add("\"EUR;CUSTOM 234.25\"", new Money(234.25m, Currency.FromCode("EUR")));
        //Add("\"EUR;ISO-4217 234.25\"", new Money(234.25m, Currency.FromCode("EUR")));
        //Add("\"EUR; 234.25\"", new Money(234.25m, Currency.FromCode("EUR")));

        // member reversed
        Add("\"234.25 EUR\"", new Money(234.25m, Currency.FromCode("EUR")));
        Add("\"234.25 BTC\"", new Money(234.25m, Currency.FromCode("BTC")));
    }
}

public class InvalidJsonV2TestData : TheoryData<string>
{
    public InvalidJsonV2TestData()
    {
        Add("\"EUR\""); // No Amount member
        Add("\"234.25\""); // No Currency member
        Add("\"EUR234.25\""); // No hard space
        Add("\"234.25EUR\""); // member reversed and no hard space
        Add("\"EUR 234.25 123\""); // Extra member
        Add("\"EUR 234.25 EUR\""); // Extra member
        Add("\"EUR 234.25 123 EUR\""); // Extra member
        Add("\"EUR 234.25 EUR 123\""); // Extra member
        Add("\"EUR EUR\""); // duplicate currency
        Add("\"234.25 234.25\""); // duplicate members
        Add("EUR 234.25"); // no quotes
        //Add("'EUR 234.25'"); // single quotes => works for Newtonsoft.Json
    }
}

public class NestedJsonV2TestData : TheoryData<string, Order>
{
    public NestedJsonV2TestData()
    {
        var order = new Order
        {
            Id = 123,
            Name = "Abc",
            Total = new Money(234.25m, Currency.FromCode("EUR"))
        };

        Add("""{ "Id": 123, "Name": "Abc", "Total": "EUR 234.25" }""", order);
        Add("""{ "id": 123, "name": "Abc", "total": "EUR 234.25" }""", order); // camelCase (System.Text.Json needs PropertyNameCaseInsensitive = true to work)

        // // Discount explicit null
        // Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": 234.25, \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as number
        // Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": \"234.25\", \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as string
    }
}

[Serializable]
public class Order
{
    public int Id { get; set; }
    public Money Total { get; set; }
    public string Name { get; set; }
}

[Serializable]
public class NullableOrder
{
    public int Id { get; set; }

    //[JsonConverter(typeof(NullableMoneyJsonConverter))]
    public Money? Total { get; set; }
    public string Name { get; set; }
}
