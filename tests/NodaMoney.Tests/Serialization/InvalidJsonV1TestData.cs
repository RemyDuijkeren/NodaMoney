using Xunit;

namespace NodaMoney.Tests.Serialization;

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
