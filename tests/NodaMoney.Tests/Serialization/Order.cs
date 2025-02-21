namespace NodaMoney.Tests.Serialization;

[Serializable]
public class Order
{
    public int Id { get; set; }
    public Money Total { get; set; }
    public string Name { get; set; }
}
