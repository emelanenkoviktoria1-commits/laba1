namespace OrderLab;

// скидка за объём действует, только если в заказе достаточно товаров
public sealed class VolumeDiscount : DiscountRule
{
    private readonly int minItems;
    private readonly double percent;

    public VolumeDiscount(int minItems, double percent)
    {
        if (minItems <= 0)
            throw new ArgumentException("Минимальное количество товаров должно быть больше нуля");
        if (percent <= 0 || percent > 100)
            throw new ArgumentException("Процент скидки должен быть больше 0 и не больше 100");

        this.minItems = minItems;
        this.percent = percent;
    }

    public double CalculateDiscount(double sum, int itemsCount)
    {
        if (itemsCount < minItems)
            return 0;
        return sum * percent / 100.0;
    }

    public override string ToString()
    {
        return $"Скидка {percent}% за {minItems}+ товаров";
    }
}
