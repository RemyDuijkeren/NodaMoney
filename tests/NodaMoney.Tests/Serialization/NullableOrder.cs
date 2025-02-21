namespace NodaMoney.Tests.Serialization;

[Serializable]
public class NullableOrder
{
    public int Id { get; set; }

    //[JsonConverter(typeof(NullableMoneyJsonConverter))]
    public Money? Total { get; set; }
    public string Name { get; set; }
}
