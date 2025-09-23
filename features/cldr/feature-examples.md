# Feature Proposal: Examples & Usage Snippets

Status: companion to proposals
Source: docs/feature-cldr-formatting-and-parsing.md

## DI Registration
```csharp
// Default to CLDR provider when the package is referenced
services.AddMoneyFormatting().UseCldr();

// Or explicitly use Culture fallback
services.AddMoneyFormatting().UseCulture();
```

## Formatting
```csharp
var options = new MoneyFormatOptions
{
    Culture = CultureInfo.GetCultureInfo("nl-NL"),
    CurrencyDisplay = CurrencyDisplay.Symbol,
    SignDisplay = SignDisplay.Accounting,
    UseCashDigits = true
};

IMoneyFormatter formatter = services.GetRequiredService<IMoneyFormatter>();
var text = formatter.Format(new Money(1234.5m, Currency.EUR), options); // "€\u00A01.234,50"
```

## Parsing
```csharp
IMoneyParser parser = services.GetRequiredService<IMoneyParser>();
if (parser.TryParse("$\u00A0(1,234.50)", CultureInfo.GetCultureInfo("en-US"), out var amount,
    new MoneyFormatOptions { SignDisplay = SignDisplay.Accounting }))
{
    // amount == -1234.50 USD
}
```

## Scoped override via MoneyContext
```csharp
using var scope = MoneyContext.CreateScope(o => o.FormattingProvider = FormattingProvider.Cldr);
```

## Notes
- UseCashDigits consults CLDR digits and can integrate with cash rounding strategies in MoneyContext.
- Narrow symbols fall back to standard symbol when CLDR narrow is missing.
