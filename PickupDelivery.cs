namespace OrderLab;

public sealed class PickupDelivery : Delivery
{
    private readonly string pickupPoint;

    public PickupDelivery(string pickupPoint) : base(0, 0)
    {
        if (string.IsNullOrWhiteSpace(pickupPoint))
            throw new ArgumentException("Пункт самовывоза не может быть пустым");

        this.pickupPoint = pickupPoint;
    }

    public override string GetMethodName()
    {
        return $"Самовывоз ({pickupPoint})";
    }

    // правило самовывоза: заказ готов на следующий день
    protected override int CalculateTermDays()
    {
        return 1;
    }
}

