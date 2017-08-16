namespace NodaMoney
{
    /// <summary>Represents Money, an amount defined in a specific Currency.</summary>
    public partial struct Money
    {
        /// <summary>Implements the operator +.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator +(Money money)
        {
            return Plus(money);
        }

        /// <summary>Implements the operator -.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator -(Money money)
        {
            return Negate(money);
        }

        /// <summary>Implements the operator ++.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator ++(Money money)
        {
            return Increment(money);
        }

        /// <summary>Implements the operator --.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result of the operator.</returns>
        public static Money operator --(Money money)
        {
            return Decrement(money);
        }

        /// <summary>Pluses the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Plus(Money money)
        {
            return money;
        }

        /// <summary>Negates the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Negate(Money money)
        {
            return new Money(-money.Amount, money.Currency);
        }

        /// <summary>Increments the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Increment(Money money)
        {
            return Add(money, new Money(money.Currency.MinorUnit, money.Currency));
        }

        /// <summary>Decrements the specified money.</summary>
        /// <param name="money">The money.</param>
        /// <returns>The result.</returns>
        public static Money Decrement(Money money)
        {
            return Subtract(money, new Money(money.Currency.MinorUnit, money.Currency));
        }
    }
}