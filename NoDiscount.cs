namespace OrderLab;

public sealed class NoDiscount : DiscountRule {
    public double CalculateDiscount(double sum, int itemsCount)
    {
        return 0;
    }

    public override string ToString()
    {
        return "Без скидки";
    }
}