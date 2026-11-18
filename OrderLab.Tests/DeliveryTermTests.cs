using Xunit;

namespace OrderLab.Tests;

// Иерархия из ЛР2: расчёт срока у курьерской, почтовой доставки и самовывоза.
// Ожидаемые значения записаны числами, а не вычисляются той же формулой, что в коде.
public class DeliveryTermTests
{
    // курьер: до 300 км в день, минимум 1 день. Границы: 300 (ещё 1 день) и 301 (уже 2 дня)
    [Theory]
    [InlineData(0.0, 1)]
    [InlineData(1.0, 1)]
    [InlineData(300.0, 1)]
    [InlineData(301.0, 2)]
    [InlineData(600.0, 2)]
    [InlineData(601.0, 3)]
    [InlineData(3000.0, 10)]
    public void курьер_везёт_до_300_км_в_день_и_не_меньше_одного_дня(double distanceKm, int expectedDays)
    {
        CourierDelivery delivery = new CourierDelivery(250.0, distanceKm);

        Assert.Equal(expectedDays, delivery.GetTermDays());
    }

    // почта: 3 дня на обработку плюс 1 день на каждые 500 км. Границы: 500 и 501
    [Theory]
    [InlineData(0.0, 3)]
    [InlineData(1.0, 4)]
    [InlineData(500.0, 4)]
    [InlineData(501.0, 5)]
    [InlineData(1500.0, 6)]
    [InlineData(1501.0, 7)]
    public void почта_тратит_три_дня_на_обработку_и_день_на_каждые_500_км(double distanceKm, int expectedDays)
    {
        PostDelivery delivery = new PostDelivery(120.0, distanceKm);

        Assert.Equal(expectedDays, delivery.GetTermDays());
    }

    [Fact]
    public void самовывоз_готов_на_следующий_день()
    {
        PickupDelivery delivery = new PickupDelivery("Пункт выдачи, ул. Тверская, 7");

        Assert.Equal(1, delivery.GetTermDays());
    }

    [Fact]
    public void самовывоз_бесплатный()
    {
        PickupDelivery delivery = new PickupDelivery("Пункт выдачи, ул. Тверская, 7");

        Assert.Equal(0.0, delivery.Cost);
    }

    [Fact]
    public void самовывоз_без_пункта_выдачи_отклоняется()
    {
        Assert.Throws<ArgumentException>(() => new PickupDelivery("   "));
    }

    // инварианты базового класса действуют для всех наследников
    [Fact]
    public void курьерская_доставка_с_отрицательной_стоимостью_отклоняется()
    {
        Assert.Throws<ArgumentException>(() => new CourierDelivery(-1.0, 100.0));
    }

    [Fact]
    public void почтовая_доставка_с_отрицательным_расстоянием_отклоняется()
    {
        Assert.Throws<ArgumentException>(() => new PostDelivery(100.0, -1.0));
    }

    // допустимая граница: нулевые стоимость и расстояние
    [Fact]
    public void курьерская_доставка_с_нулевыми_стоимостью_и_расстоянием_допустима()
    {
        CourierDelivery delivery = new CourierDelivery(0.0, 0.0);

        Assert.Equal(1, delivery.GetTermDays());
    }

    [Fact]
    public void описание_доставки_начинается_с_названия_способа_и_срока()
    {
        CourierDelivery delivery = new CourierDelivery(250.0, 600.0);

        Assert.StartsWith("Курьерская доставка: срок 2 дн.", delivery.ToString(), StringComparison.Ordinal);
    }

    
}
