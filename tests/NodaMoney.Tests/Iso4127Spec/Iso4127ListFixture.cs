using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace NodaMoney.Tests.Iso4127Spec;

public class Iso4127ListFixture
{
    public Iso4127Currency[] Currencies { get; private set; }
    public DateTime PublishDate { get; private set; }

    public Iso4127ListFixture()
    {
        const string fileName = "iso4127.xml";
        const string listOneUrl = "https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xml";

        // Download ISO-4127 XML as file
        using HttpClient client = new();
        using (Stream contentStream = client.GetStreamAsync(listOneUrl).GetAwaiter().GetResult())
        {
            using FileStream fileStream = new(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            contentStream.CopyTo(fileStream);
        }

        // Parse XML
        var document = XDocument.Load(fileName);

        Currencies = document.Element("ISO_4217")!.Element("CcyTbl")!.Elements("CcyNtry")
                             .Select(e =>
                                 new Iso4127Currency
                                 {
                                     CountryName = e.Element("CtryNm")!.Value,
                                     CurrencyName = e.Element("CcyNm")?.Value,
                                     Currency = e.Element("Ccy")?.Value,
                                     CurrencyNumber = e.Element("CcyNbr")?.Value,
                                     CurrencyMinorUnits = e.Element("CcyMnrUnts")?.Value
                                 })
                             .Where(a => !string.IsNullOrEmpty(a.Currency)) // ignore currencies without a currency name
                             .ToArray();

        PublishDate = DateTime.Parse(document.Element("ISO_4217")!.Attribute("Pblshd")!.Value);
    }
}
