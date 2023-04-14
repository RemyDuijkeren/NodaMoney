using System.Globalization;

namespace NodaMoney.Tests.Helpers
{
    /// <summary>Names for <see cref="CultureInfo" /> instances.</summary>
    public static class CultureNames
    {
        // '.': CurrencyDecimalSeparator
        // ',': CurrencyGroupSeparator
        public const string Invariant = "";

        public const string UnitedStatesEnglish = "en-US";
        public const string NetherlandsDutch = "nl-NL";
        public const string BelgiumDutch = "nl-BE";
        public const string BelgiumFrench = "fr-BE";
        public const string BrazilPortuguese = "pt-BR";
        public const string JapanJapanese = "ja-JP";
        public const string ArgentinaSpanish = "es-AR";
        public const string ChinaMainlandChinese = "zh-CN";

        // '\u00A0': Unicode Character 'NO-BREAK SPACE' (U+00A0), used as CurrencyGroupSeparator in fr-FR
        public const string FranceFrench = "fr-FR";

        // '’': Unicode Character 'RIGHT SINGLE QUOTATION MARK' (U+2019), used as CurrencyGroupSeparator in de-CH
        public const string SwitzerlandGerman = "de-CH";
    }
}
