namespace OrderLab;

// интерфейс умеет: правило скидки order работает только с ним
// и не знает, какие именно правила ему передали
public interface DiscountRule
{
    // возвращает сумму скидки в рублях для заказа на сумму sum с itemsCount товарами
    double CalculateDiscount(double sum, int itemsCount);
}
