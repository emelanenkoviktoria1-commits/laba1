using Xunit;

namespace OrderLab.Tests;

// Инвариант: номер заказа уникален и задаётся один раз при создании.
// Счётчик заказов статический, поэтому в тестах сравниваются разности, а не абсолютные значения.
public class OrderNumberTests
{
    private static Customer NewCustomer()
    {
        return new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
    }

    [Fact]
    public void номер_следующего_заказа_больше_номера_предыдущего()
    {
        Order first = new Order(NewCustomer(), 100.0);
        Order second = new Order(NewCustomer(), 100.0);

        Assert.True(second.Number > first.Number);
    }

    [Fact]
    public void номера_двадцати_заказов_уникальны()
    {
        HashSet<int> numbers = new HashSet<int>();

        for (int i = 0; i < 20; i++)
            numbers.Add(new Order(NewCustomer(), 100.0).Number);

        Assert.Equal(20, numbers.Count);
    }

    [Fact]
    public void счётчик_заказов_растёт_ровно_на_один_при_создании()
    {
        int before = Order.GetOrdersCreatedCount();

        new Order(NewCustomer(), 100.0);

        Assert.Equal(before + 1, Order.GetOrdersCreatedCount());
    }

    [Fact]
    public void неудачное_создание_заказа_не_тратит_номер()
    {
        int before = Order.GetOrdersCreatedCount();

        Record.Exception(() => new Order(NewCustomer(), 0.0));

        Assert.Equal(before, Order.GetOrdersCreatedCount());
    }

    [Fact]
    public void неудачное_создание_заказа_с_неверной_позицией_не_тратит_номер()
    {
        int before = Order.GetOrdersCreatedCount();

        Record.Exception(() => new Order(NewCustomer(), "Чехол", 0, 500.0));

        Assert.Equal(before, Order.GetOrdersCreatedCount());
    }
}
