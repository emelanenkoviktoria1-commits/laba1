namespace OrderLab;

// Признак 1: способ доставки. Интерфейс "умеет", три реализации.
public interface DeliveryMethod
{
    int BaseDays();
    double BaseCost();
}

public sealed class CourierMethod : DeliveryMethod
{
    public int BaseDays() { return 2; }
    public double BaseCost() { return 300; }
    public override string ToString() { return "курьер"; }
}

public sealed class PickupMethod : DeliveryMethod
{
    public int BaseDays() { return 1; }
    public double BaseCost() { return 0; }
    public override string ToString() { return "самовывоз"; }
}

public sealed class PostMethod : DeliveryMethod
{
    public int BaseDays() { return 6; }
    public double BaseCost() { return 150; }
    public override string ToString() { return "почта"; }
}
