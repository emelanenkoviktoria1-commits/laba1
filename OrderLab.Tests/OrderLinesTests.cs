using Xunit;

namespace OrderLab.Tests;

// Композиция Order -> OrderLine (ЛР2): позиции создаются внутри заказа и не протекают наружу.
public class OrderLinesTests
{
    private static Customer NewCustomer()
    {
        return new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
    }

    private static Order OrderWithThreeLines()
    {
        Order order = new Order(NewCustomer(), "Ноутбук", 1, 60000.0);
        order.AddLine("Мышь", 2, 1500.0);
        order.AddLine("Коврик", 3, 500.0);
        return order;
    }

    [Fact]
    public void сумма_заказа_равна_сумме_стоимостей_позиций()
    {
        Order order = OrderWithThreeLines();

        Assert.Equal(64500.0, order.Sum);   // 60000 + 3000 + 1500
    }

    [Fact]
    public void количество_товаров_равно_сумме_количеств_по_позициям()
    {
        Order order = OrderWithThreeLines();

        Assert.Equal(6, order.GetItemsCount());   // 1 + 2 + 3
    }

    // допустимая граница: одна штука; первое недопустимое: ноль
    [Fact]
    public void позиция_в_одну_штуку_допустима()
    {
        Order order = new Order(NewCustomer(), "Ноутбук", 1, 100.0);

        Assert.Equal(1, order.GetItemsCount());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void позиция_с_нулевым_или_отрицательным_количеством_отклоняется(int count)
    {
        Order order = new Order(NewCustomer(), "Ноутбук", 1, 100.0);

        Assert.Throws<ArgumentException>(() => order.AddLine("Мышь", count, 10.0));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void позиция_с_нулевой_или_отрицательной_ценой_отклоняется(double price)
    {
        Order order = new Order(NewCustomer(), "Ноутбук", 1, 100.0);

        Assert.Throws<ArgumentException>(() => order.AddLine("Мышь", 1, price));
    }

    [Fact]
    public void позиция_с_пустым_названием_отклоняется()
    {
        Order order = new Order(NewCustomer(), "Ноутбук", 1, 100.0);

        Assert.Throws<ArgumentException>(() => order.AddLine("   ", 1, 10.0));
    }

    [Fact]
    public void в_оплаченный_заказ_позицию_добавить_нельзя()
    {
        Order order = OrderWithThreeLines();
        order.Pay();

        Assert.Throws<InvalidOperationException>(() => order.AddLine("Чехол", 1, 500.0));
    }

    [Fact]
    public void список_позиций_наружу_отдаётся_копией()
    {
        Order order = OrderWithThreeLines();
        string[] info = order.GetLinesInfo();

        info[0] = "подмена";

        Assert.NotEqual("подмена", order.GetLinesInfo()[0]);
    }


}
