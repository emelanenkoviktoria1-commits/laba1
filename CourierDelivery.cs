namespace OrderLab;

public sealed class CourierDelivery : Delivery
{  // курьерская доставка "является" доставкой: подставляется везде, где ждут доставку

    public CourierDelivery(double cost, double distanceKm) : base(cost, distanceKm)
    {
    }

    public override string GetMethodName()
    {
        return "Курьерская доставка";
    }

    // правило курьера: проезжает до 300 км в день, минимум один день
    protected override int CalculateTermDays()
    {
        int days = (int)Math.Ceiling(DistanceKm / 300.0);
        if (days < 1)
            days = 1;
        return days;
    }

}



