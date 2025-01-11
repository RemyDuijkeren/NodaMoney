using System.Globalization;

namespace NodaMoney;

/// <summary>Defines a custom currency that is new or based on another currency.</summary>
public class CurrencyInfoBuilder
{
    const string InvalidCurrencyMessage = "Currency code should only exist out of three capital letters";
    private byte _decimalDigits;
    private short _number;

    /// <summary>Initializes a new instance of the <see cref="CurrencyInfoBuilder"/> class.</summary>
    /// <param name="code">The code of the currency, normally the three-character ISO 4217 currency code.</param>
    /// <exception cref="ArgumentNullException"><paramref name="code"/> is <see langword="null" /> or empty.</exception>
    public CurrencyInfoBuilder(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        if (code.Length != 3)
            throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

        // check that code exist out of only out of three capital letters
        foreach (var c in code)
            if (c is < 'A' or > 'Z')
                throw new ArgumentException(InvalidCurrencyMessage, nameof(code));

        Code = code;
        IsIso4217 = false;
    }

    /// <summary>Gets or sets the english name of the currency.</summary>
    public string EnglishName { get; set; }

    /// <summary>Gets or sets the currency sign.</summary>
    public string Symbol { get; set; }

    /// <summary>Gets or sets the numeric ISO 4217 currency code.</summary>
    public string NumericCode
    {
        get => _number.ToString("D3", CultureInfo.InvariantCulture);
        set
        {
            if (!short.TryParse(value, out short result))
            {
                throw new ArgumentException("Not a number!");
            }

            _number = result;
        }
    }

    /// <summary>Gets or sets the number of digits after the decimal separator.</summary>
    public double DecimalDigits
    {
        get => _decimalDigits;
        set
        {
            // TODO: Fix constraints
            if (value is < 0 or > 13)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "DecimalDigits must greater or equal to zero and smaller or equal to 13, or -1 if not applicable!");

            _decimalDigits = (byte)value;
        }
    }

    /// <summary>Gets the code of the currency, normally a three-character ISO 4217 currency code.</summary>
    public string Code { get; }

    /// <summary>Gets or sets the date when the currency is valid from.</summary>
    /// <value>The date from which the currency is valid.</value>
    public DateTime? ValidFrom { get; set; }

    /// <summary>Gets or sets the date when the currency is valid to.</summary>
    /// <value>The date until the currency is valid.</value>
    public DateTime? ValidTo { get; set; }

    public bool IsIso4217 { get; set; }

    /// <summary>Reconstitutes a <see cref="CurrencyInfoBuilder"/> object from a specified XML file that contains a
    /// representation of the object.</summary>
    /// <param name="xmlFileName">A file name that contains the XML representation of a <see cref="CurrencyInfoBuilder"/> object.</param>
    /// <returns>A new object that is equivalent to the information stored in the <i>xmlFileName</i> parameter.</returns>
    public static CurrencyInfoBuilder CreateFromLdml(string xmlFileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>Unregisters the specified currency code from the current AppDomain and returns it.</summary>
    /// <param name="code">The name of the currency to unregister.</param>
    /// <returns>An instance of the type <see cref="Currency"/>.</returns>
    /// <exception cref="ArgumentException">code specifies a currency that is not found in the given namespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="code" /> is <see langword="null" /> or empty.</exception>
    public static CurrencyInfo Unregister(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        // TODO: fix for other namespaces
        var currencyInfo = CurrencyInfo.FromCode(code);
        if (CurrencyRegistry.TryRemove(currencyInfo))
            return currencyInfo;

        throw new InvalidCurrencyException($"Can't unregister the currency {code} because it is not registered!");
    }

    /// <summary>Builds the current <see cref="CurrencyInfoBuilder"/> object as a custom currency.</summary>
    /// <returns>A <see cref="CurrencyInfo"/> instance that is build.</returns>
    //// <exception cref="InvalidOperationException">The current CurrencyBuilder object has a property that must be set before the currency can be registered.</exception>
    public CurrencyInfo Build()
    {
        // throw new InvalidOperationException("The current CurrencyBuilder object has a property that must be set before the currency can be registered.");
        if (string.IsNullOrWhiteSpace(Symbol))
            Symbol = CurrencyInfo.GenericCurrencySign;


        return new CurrencyInfo(Code, _number, (MinorUnit)_decimalDigits, EnglishName, Symbol)
        {
            IntroducedOn = ValidFrom,
            ExpiredOn = ValidTo,
            IsIso4217 = IsIso4217
        };
    }

    /// <summary>Registers the current <see cref="CurrencyInfoBuilder"/> object as a custom currency for the current AppDomain.</summary>
    /// <returns>A <see cref="CurrencyInfo"/> instance that is build and registered.</returns>
    /// <exception cref="InvalidOperationException">
    ///     <para>The custom currency is already registered -or-.</para>
    ///     <para>The current CurrencyBuilder object has a property that must be set before the currency can be registered.</para>
    /// </exception>
    public CurrencyInfo Register()
    {
        CurrencyInfo currency = Build();
        if (CurrencyRegistry.TryAdd(currency) == false)
            throw new InvalidCurrencyException($"The currency {Code} is already registered.");

        return currency;
    }

    /// <summary>Writes an XML representation of the current <see cref="CurrencyInfoBuilder"/> object to the specified file.</summary>
    /// <param name="fileName">The name of a file to contain the XML representation of this custom currency.</param>
    /// <remarks>
    ///     <para>
    ///     The Save method writes the current <see cref="CurrencyInfoBuilder"/> object to the file specified by the
    ///     filename parameter in an XML format called Locale Data Markup Language (LDML) version 1.1.
    ///     The <see cref="CreateFromLdml"/> method performs the reverse operation of the Save method.
    ///     </para>
    ///     <para>
    ///     For information about the format of an LDML file, see the CreateFromLdml method. For information about the LDML
    ///     standard, see <see href='http://go.microsoft.com/fwlink/p/?LinkId=252840'>Unicode Technical Standard #35, "Locale
    ///     Data Markup Language (LDML)"</see> on the Unicode Consortium website.
    ///     </para>
    /// </remarks>
    public void Save(string fileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>Sets the properties of the current <see cref="CurrencyInfoBuilder"/> object with the corresponding properties of
    /// the specified <see cref="Currency"/> object, except for the code and namespace.</summary>
    /// <param name="currency">The object whose properties will be used.</param>
    public void LoadDataFromCurrencyInfo(CurrencyInfo currencyInfo)
    {
        if (currencyInfo == null)
            throw new ArgumentNullException(nameof(currencyInfo));

        IsIso4217 = currencyInfo.IsIso4217;
        EnglishName = currencyInfo.EnglishName;
        Symbol = currencyInfo.Symbol;
        NumericCode = currencyInfo.NumericCode;
        DecimalDigits = currencyInfo.DecimalDigits;
        ValidFrom = currencyInfo.IntroducedOn;
        ValidTo = currencyInfo.ExpiredOn;
    }
}
