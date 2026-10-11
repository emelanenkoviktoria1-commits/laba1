namespace OrderLab;

// Признак 2: скорость доставки. Интерфейс "умеет", две реализации.
public interface DeliverySpeed
{
    int AdjustDays(int days);
    double AdjustCost(double cost);
}

public sealed class StandardSpeed : DeliverySpeed
{
    public int AdjustDays(int days) { return days; }
    public double AdjustCost(double cost) { return cost; }
    public override string ToString() { return "обычная"; }
}

// экспресс: срок вдвое меньше (с округлением вверх, не меньше 1 дня), цена в 1,5 раза выше
public sealed class ExpressSpeed : DeliverySpeed
{
    public int AdjustDays(int days) { return (days + 1) / 2; }
    public double AdjustCost(double cost) { return cost * 1.5; }
    public override string ToString() { return "экспресс"; }
}
