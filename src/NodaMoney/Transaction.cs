using System;
using System.Collections.Generic;
using System.Text;

namespace NodaMoney
{
    public class Transaction
    {
        Money Value { get; set; }

        ExchangeRate ExchangeRate { get; set; }

        Money Tax { get; set; }

        Money Discount { get; set; }
    }
}
