using System;

namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
    {
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(decimal amount)
        {
            return new Money(amount, Currency.FromCode("EUR"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("EUR"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
        public static Money Euro(double amount)
        {
            return new Money(amount, Currency.FromCode("EUR"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</para></remarks>
        public static Money Euro(double amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("EUR"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(long amount)
        {
            return new Money(amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money Euro(ulong amount)
        {
            return new Money(amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(decimal amount)
        {
            return new Money(amount, Currency.FromCode("USD"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("USD"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
        public static Money USDollar(double amount)
        {
            return new Money(amount, Currency.FromCode("USD"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</para></remarks>
        public static Money USDollar(double amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("USD"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(long amount)
        {
            return new Money(amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money USDollar(ulong amount)
        {
            return new Money(amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(decimal amount)
        {
            return new Money(amount, Currency.FromCode("JPY"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yens.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("JPY"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
        public static Money Yen(double amount)
        {
            return new Money(amount, Currency.FromCode("JPY"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</para></remarks>
        public static Money Yen(double amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("JPY"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(long amount)
        {
            return new Money(amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money Yen(ulong amount)
        {
            return new Money(amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(decimal amount)
        {
            return new Money(amount, Currency.FromCode("GBP"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("GBP"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
        public static Money PoundSterling(double amount)
        {
            return new Money(amount, Currency.FromCode("GBP"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</para></remarks>
        public static Money PoundSterling(double amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("GBP"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(long amount)
        {
            return new Money(amount, Currency.FromCode("GBP"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money PoundSterling(ulong amount)
        {
            return new Money(amount, Currency.FromCode("GBP"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        public static Money Yuan(decimal amount)
        {
            return new Money(amount, Currency.FromCode("CNY"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        public static Money Yuan(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("CNY"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
        /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
        /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
        /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
        public static Money Yuan(double amount)
        {
            return new Money(amount, Currency.FromCode("CNY"));
        }

#if !PORTABLE40
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
        /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
        /// <para>The amount will be rounded to the number of decimal digits of the specified currency
        /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</para></remarks>
        public static Money Yuan(double amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("CNY"), rounding);
        }
#endif

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        public static Money Yuan(long amount)
        {
            return new Money(amount, Currency.FromCode("CNY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
        /// <param name="amount">The Amount of money in Chinese Yuan.</param>
        /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money Yuan(ulong amount)
        {
            return new Money(amount, Currency.FromCode("CNY"));
        }
    }
}