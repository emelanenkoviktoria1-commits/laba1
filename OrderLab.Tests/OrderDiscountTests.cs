using Xunit;

namespace OrderLab.Tests;

// Инварианты скидки: процент больше 0 и не больше MaxDiscountPercent (30),
// скидку можно применить только к новому заказу, сумма остаётся больше нуля.
public class OrderDiscountTests
{
    private static Order OrderOf1000()
    {
        Customer customer = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        return new Order(customer, 1000.0);
    }

    [Fact]
    public void скидка_10_процентов_уменьшает_сумму_с_1000_до_900()
    {
        Order order = OrderOf1000();

        order.ApplyDiscount(10);

        Assert.Equal(900.0, order.Sum);
    }

    // допустимая граница: ровно максимальный процент
    [Fact]
    public void скидка_ровно_30_процентов_допустима()
    {
        Order order = OrderOf1000();

        order.ApplyDiscount(30);

        Assert.Equal(700.0, order.Sum);
    }

    // первое недопустимое значение и дальше
    [Theory]
    [InlineData(30.01)]
    [InlineData(31.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void скидка_выше_30_процентов_отклоняется(double percent)
    {
        Order order = OrderOf1000();

        Assert.Throws<ArgumentException>(() => order.ApplyDiscount(percent));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-30.0)]
    public void нулевая_и_отрицательная_скидка_отклоняется(double percent)
    {
        Order order = OrderOf1000();

        Assert.Throws<ArgumentException>(() => order.ApplyDiscount(percent));
    }

    [Fact]
    public void сумма_скидки_урезается_до_заданного_максимума()
    {
        Order order = OrderOf1000();

        order.ApplyDiscount(20, 100);   // 20 % это 200 руб., но максимум 100

        Assert.Equal(900.0, order.Sum);
    }

    [Fact]
    public void скидка_к_оплаченному_заказу_отклоняется()
    {
        Order order = OrderOf1000();
        order.Pay();

        Assert.Throws<InvalidOperationException>(() => order.ApplyDiscount(10));
    }

    [Fact]
    public void после_отклонённой_скидки_сумма_не_меняется()
    {
        Order order = OrderOf1000();

        Record.Exception(() => order.ApplyDiscount(50));

        Assert.Equal(1000.0, order.Sum);
    }

    [Fact]
    public void скидка_10_процентов_на_1000_даёт_901()
    {
        Customer customer = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        Order order = new Order(customer, 1000.0);

        order.ApplyDiscount(10);

        Assert.Equal(901.0, order.Sum);
    }
}
