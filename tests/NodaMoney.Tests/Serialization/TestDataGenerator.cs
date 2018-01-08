using System;
using System.Collections.Generic;
using Xunit;

namespace NodaMoney.Tests.Serialization
{
    public class ValidJsonTestData : TheoryData<string, Money>
    {
        public ValidJsonTestData()
        {
            var money = new Money(234.25m, Currency.FromCode("EUR"));

            Add("{ \"Amount\": 234.25, \"Currency\": \"EUR\" }", money); // PascalCase, Amount as number
            Add("{ \"Currency\": \"EUR\", \"Amount\": 234.25 }", money); // PascalCase, Amount as number, Reversed members
            Add("{ \"Amount\": \"234.25\", \"Currency\": \"EUR\" }", money); // PascalCase, Amount as string
            Add("{ \"Currency\": \"EUR\", \"Amount\": \"234.25\" }", money); // PascalCase, Amount as string, Reversed members
            Add("{ \"amount\": 234.25, \"currency\": \"EUR\" }", money); // camelCase, Amount as number
            Add("{ \"currency\": \"EUR\", \"amount\": 234.25 }", money); // camelCase, Amount as number, Reversed members
            Add("{ \"amount\": \"234.25\", \"currency\": \"EUR\" }", money); // camelCase, Amount as string
            Add("{ \"currency\": \"EUR\", \"amount\": \"234.25\" }", money); // camelCase, Amount as string, Reversed members

            // Members no quotation marks
            Add("{ Amount: 234.25, Currency: \"EUR\" }", money); // PascalCase, Amount as number
            Add("{ Currency: \"EUR\", Amount: 234.25 }", money); // PascalCase, Amount as number, Reversed members
            Add("{ Amount: \"234.25\", Currency: \"EUR\" }", money); // PascalCase, Amount as string
            Add("{ Currency: \"EUR\", Amount: \"234.25\" }", money); // PascalCase, Amount as string
            Add("{ amount: 234.25, currency: \"EUR\" }", money); // camelCase, Amount as number
            Add("{ currency: \"EUR\", amount: 234.25 }", money); // camelCase, Amount as number, Reversed members
            Add("{ amount: \"234.25\", currency: \"EUR\" }", money); // camelCase, Amount as string, Members no quotation marks
            Add("{ currency: \"EUR\", amount: \"234.25\" }", money); // camelCase, Amount as string, Reversed members

            // Members no quotation marks, Values single quotes
            Add("{ Amount: 234.25, Currency: 'EUR' }", money); // PascalCase, Amount as number, 
            Add("{ Currency: 'EUR', Amount: 234.25 }", money); // PascalCase, Amount as number, Reversed members
            Add("{ Amount: '234.25', Currency: 'EUR' }", money); // PascalCase, Amount as string
            Add("{ Currency: 'EUR', Amount: '234.25' }", money); // PascalCase, Amount as string, Reversed members
            Add("{ amount: 234.25, currency: 'EUR' }", money); // camelCase, Amount as number
            Add("{ currency: 'EUR', amount: 234.25 }", money); // camelCase, Amount as number, Reversed members
            Add("{ amount: '234.25', currency: 'EUR' }", money); // camelCase, Amount as string
            Add("{ currency: 'EUR', amount: '234.25' }", money); // camelCase, Amount as string, Reversed members
        }
    }

    public class InvalidJsonTestData : TheoryData<string>
    {
        public InvalidJsonTestData()
        {
            Add("{ \"Amount\": 234.25 }"); // PascalCase, Amount as number, No Currency member
            Add("{ \"Currency\": \"EUR\" }"); // PascalCase, No Amount member
            Add("{ \"Amount\": \"234.25\" }"); // PascalCase, Amount as string, No Currency member
            Add("{ \"amount\": 234.25 }"); // camelCase, Amount as number, No Currency member
            Add("{ \"currency\": \"EUR\" }"); // camelCase, No Amount member

            // Members no quotation marks
            Add("{ Amount: 234.25 }"); // PascalCase, Amount as number, No Currency member 
            Add("{ Currency: \"EUR\" }"); // PascalCase, No Amount member
            Add("{ Amount: \"234.25\" }"); // PascalCase, Amount as string, No Currency member
            Add("{ amount: 234.25 }"); // camelCase, Amount as number, No Currency member
            Add("{ currency: \"EUR\" }"); // camelCase, No Amount member

            // Members no quotation marks, Values single quotes
            Add("{ Currency: 'EUR' }"); // PascalCase, No Amount member, 
            Add("{ Amount: '234.25' }"); // PascalCase, Amount as string, No Currency member
            Add("{ currency: 'EUR' }"); // camelCase, No Amount member

            Add("{ \"Amount\": \"ABC\", \"Currency\": \"EUR\" }"); // => formatexception without telling wich member
        }
    }

    public class NestedJsonTestData : TheoryData<string, Order>
    {
        public NestedJsonTestData()
        {
            var order = new Order
            {
                Id = 123,
                Name = "Abc",
                Price = new Money(234.25m, Currency.FromCode("EUR"))
            };

            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": 234.25, \"Currency\": \"EUR\" } }", order); // Amount as number
            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": \"234.25\", \"Currency\": \"EUR\" } }", order); // Amount as string

            // Reversed mebers
            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Currency\": \"EUR\", \"Amount\": 234.25 } }", order); // Amount as number
            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Currency\": \"EUR\", \"Amount\": \"234.25\" } }", order); // Amount as string

            // camelCase
            Add("{ \"id\": 123, \"name\": \"Abc\", \"price\": { \"amount\": 234.25, \"currency\": \"EUR\" } }", order); // Amount as number
            Add("{ \"id\": 123, \"name\": \"Abc\", \"price\": { \"amount\": \"234.25\", \"currency\": \"EUR\" } }", order); // Amount as string

            // Discount explicit null
            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": 234.25, \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as number
            Add("{ \"Id\": 123, \"Name\": \"Abc\", \"Price\": { \"Amount\": \"234.25\", \"Currency\": \"EUR\" }, \"Discount\": null }", order); // Amount as string
        }
    }

    [Serializable]
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Money Price { get; set; }
        public Money? Discount { get; set; }
    }
}