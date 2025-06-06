using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NodaMoney.Tests.Serialization.XmlSerializationSpec;

// old format v1 : <Money Amount="765.43" Currency="USD" />
// new format v2: <money currency="USD">765.43</money>.
// Last option is better because it separates the currency and amount into two distinct parts: an attribute and the text content.
// This can make it easier to parse programmatically, as you can directly access the currency and amount without needing to split a string.
// However, it is slightly more complex in terms of XML structure.
// In terms of XML best practices, attributes are generally used to provide additional information about an element, while the
// text content of the element is the primary data. In this case, the amount of money could be considered the primary data,
// and the currency could be considered additional information, which would suggest that option 2 is more in line with XML best practices.

public class XmlSerializationHelper
{
    public static string SerializeToXml(object source)
    {
        var settings = new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = true
        };
        using var stream = new StringWriter();
        using var writer = XmlWriter.Create(stream, settings);

        var xmlSerializer = new XmlSerializer(source.GetType());

        // Suppress namespace declarations
        var emptyNamespaces = new XmlSerializerNamespaces();
        emptyNamespaces.Add("", "");

        xmlSerializer.Serialize(writer, source, emptyNamespaces);

        return stream.ToString();
    }

    public static T DeserializeFromXml<T>(string xml)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stream = new StringReader(xml);
        return (T)xmlSerializer.Deserialize(stream);
    }

    public static T Clone<T>(object source) => DeserializeFromXml<T>(SerializeToXml(source));
}
