using Xunit;

namespace NodaMoney.Tests.Serialization;

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
