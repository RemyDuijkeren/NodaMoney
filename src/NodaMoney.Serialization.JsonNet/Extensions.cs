using System;
using Newtonsoft.Json;

namespace NodaMoney.Serialization.JsonNet
{
    public static class Extensions
    {
        public static JsonSerializerSettings ConfigureForNodaMoney(this JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            settings.Converters.Add(new MoneyJsonConverter());
            settings.Converters.Add(new CurrencyJsonConverter());
            return settings;
        }
    }
}
