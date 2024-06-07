namespace NodaMoney;
/// <summary>A unit of exchange of value, a currency of <see cref="Money" />.</summary>
/// <remarks>See http://en.wikipedia.org/wiki/Currency and
/// https://en.wikipedia.org/wiki/List_of_circulating_currencies</remarks>
/// <param name="Code">The (ISO-4217) three-character code of the currency.</param>
/// <param name="Number">The (ISO-4217) number of the currency.</param>
/// <param name="MinorUnit">The minor unit, as an exponent of base 10, by which the currency unit can be divided in.</param>
/// <param name="EnglishName">The english name of the currency</param>
/// <param name="Symbol">The currency symbol.</param>
record CurrencyInfo(
    string Code,
    short Number,
    MinorUnit MinorUnit,
    string EnglishName,
    string Symbol)
{
    public CurrencyList CurrencyList { get; init; } = CurrencyList.Iso4217;
    public DateTime? ValidTo { get; init; }
    public DateTime? ValidFrom { get; init; }
    public string NativeName { get; init; } = EnglishName;
    public string FractionalUnit { get; init; } = "Cent";

    /// <summary>Gets a value indicating whether currency is valid.</summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    public bool IsValid => IsValidOn(DateTime.Today);

    /// <summary>Check a value indication whether currency is valid on a given date.</summary>
    /// <param name="date">The date on which the Currency should be valid.</param>
    /// <returns><c>true</c> when the date is within the valid range of this currency; otherwise <c>false</c>.</returns>
    public bool IsValidOn(DateTime date) =>
        (!ValidFrom.HasValue || ValidFrom <= date) &&
        (!ValidTo.HasValue || ValidTo >= date);

    // Thread.CurrentThread.CurrentCulture = new CurrentInfo("")
    // Thread.CurrentThread.CurrentCurrency = new CurrencyInfo("");
    //
    // CultureInfo.DefaultThreadCurrentCurrency = CurrencyInfo;
    //
    // var x = CurrencyInfo.CurrentCurrency;
    //
    // IsHistoric, ReplacedBy, Replaces, IntroductionOn, AlternativeSymbol, Locals (NL, EN), IsIso4217
    // Order/Priority/Weight=1,0.8,0.6 = q-factor weighting (value between 0 and 1) = Any value placed in an order of preference expressed using a relative quality value called weight.
}
