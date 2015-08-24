using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodaMoney
{
    internal class CurrencyNamespace : Dictionary<string, Currency> // ConcurrentDictionary<string, Currency>
    {
        public string Name { get; set; }
    }
}
