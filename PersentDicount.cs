namespace OrderLab;

public sealed class PercentDiscount : DiscountRule
{
    private readonly double percent;

    public PercentDiscount(double percent)
    {
        if (percent <= 0 || percent > 100)
            throw new ArgumentException("Процент скидки должен быть больше 0 и не больше 100");

        this.percent = percent;
    }

    public double CalculateDiscount(double sum, int itemsCount)
    {
        return sum * percent / 100.0;
    }

    public override string ToString()
    {
        return $"Скидка {percent}% от суммы";
    }
}
