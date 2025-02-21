namespace NodaMoney.Tests.Serialization;

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
