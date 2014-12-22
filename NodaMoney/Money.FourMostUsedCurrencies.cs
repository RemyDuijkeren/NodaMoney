using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodaMoney
{
    public partial struct Money
    {
        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(decimal amount)
        {
            return new Money(amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("EUR"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(double amount)
        {
            return new Money((decimal)amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(double amount, MidpointRounding rounding)
        {
            return new Money((decimal)amount, Currency.FromCode("EUR"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        public static Money Euro(long amount)
        {
            return new Money((decimal)amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money Euro(ulong amount)
        {
            return new Money((decimal)amount, Currency.FromCode("EUR"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(decimal amount)
        {
            return new Money(amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("USD"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(double amount)
        {
            return new Money((decimal)amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(double amount, MidpointRounding rounding)
        {
            return new Money((decimal)amount, Currency.FromCode("USD"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        public static Money USDollar(long amount)
        {
            return new Money((decimal)amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
        /// <param name="amount">The Amount of money in US dollar.</param>
        /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money USDollar(ulong amount)
        {
            return new Money((decimal)amount, Currency.FromCode("USD"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(decimal amount)
        {
            return new Money(amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("JPY"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(double amount)
        {
            return new Money((decimal)amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(double amount, MidpointRounding rounding)
        {
            return new Money((decimal)amount, Currency.FromCode("JPY"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        public static Money Yen(long amount)
        {
            return new Money((decimal)amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
        /// <param name="amount">The Amount of money in Japanese Yen.</param>
        /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money Yen(ulong amount)
        {
            return new Money((decimal)amount, Currency.FromCode("JPY"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(decimal amount)
        {
            return new Money(amount, Currency.FromCode("GBP"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in euro.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(decimal amount, MidpointRounding rounding)
        {
            return new Money(amount, Currency.FromCode("GBP"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(double amount)
        {
            return new Money((decimal)amount, Currency.FromCode("GBP"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <param name="rounding">The rounding.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(double amount, MidpointRounding rounding)
        {
            return new Money((decimal)amount, Currency.FromCode("GBP"), rounding);
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        public static Money PoundSterling(long amount)
        {
            return new Money((decimal)amount, Currency.FromCode("GBP"));
        }

        /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
        /// <param name="amount">The Amount of money in Pound Sterling.</param>
        /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
        [CLSCompliant(false)]
        public static Money PoundSterling(ulong amount)
        {
            return new Money((decimal)amount, Currency.FromCode("GBP"));
        }
    }
}
