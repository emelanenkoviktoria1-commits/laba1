namespace OrderLab;

// Почтовая доставка "является" доставкой: медленнее курьера.
public sealed class PostDelivery : Delivery
{
    public PostDelivery(double cost, double distanceKm) : base(cost, distanceKm)
    {
    }

    public override string GetMethodName()
    {
        return "Почтовая доставка";
    }

    // правило почты: 3 дня на обработку плюс 1 день на каждые 500 км
    protected override int CalculateTermDays()
    {
        return 3 + (int)Math.Ceiling(DistanceKm / 500.0);
    }
}