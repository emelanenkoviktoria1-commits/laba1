using Xunit;

namespace OrderLab.Tests;

// Инвариант: сумма заказа строго больше нуля.
public class OrderSumTests
{
    private static Customer NewCustomer()
    {
        return new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
    }

    [Fact]
    public void заказ_с_положительной_суммой_создаётся()
    {
        Order order = new Order(NewCustomer(), 5000.0);

        Assert.Equal(5000.0, order.Sum);
    }

    // допустимая граница: наименьшая положительная сумма (одна копейка)
    [Fact]
    public void заказ_на_одну_копейку_создаётся()
    {
        Order order = new Order(NewCustomer(), 0.01);

        Assert.Equal(0.01, order.Sum);
    }

    // первое недопустимое значение: ноль, дальше отрицательные
    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.01)]
    [InlineData(-1.0)]
    [InlineData(-1000.0)]
    public void заказ_с_нулевой_или_отрицательной_суммой_отклоняется(double sum)
    {
        Assert.Throws<ArgumentException>(() => new Order(NewCustomer(), sum));
    }

    [Fact]
    public void новый_заказ_получает_статус_новый()
    {
        Order order = new Order(NewCustomer(), 100.0);

        Assert.Equal(OrderStatus.New, order.Status);
    }

    [Fact]
    public void заказ_без_заказчика_отклоняется()
    {
        Assert.Throws<ArgumentNullException>(() => new Order(null!, 100.0));
    }

    [Fact]
    public void упрощённый_конструктор_создаёт_заказ_на_минимальную_сумму_один_рубль()
    {
        Order order = new Order(NewCustomer());

        Assert.Equal(1.0, order.Sum);
    }
}
