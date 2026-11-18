using OrderLab.Tests.Doubles;
using Xunit;

namespace OrderLab.Tests;

// Задание 3. Тесты службы с ДУБЛЁРАМИ, написанными вручную:
// FixedTimeSource (заглушка времени), FixedRandomSource (заглушка случайности),
// InMemoryOrderRepository (фейк хранилища). Системные часы и генератор случайных чисел не используются.
public class OrderServiceTests
{
    private static Customer NewCustomer()
    {
        return new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
    }

    private static Order NewOrder()
    {
        return new Order(NewCustomer(), 1000.0);
    }

    private static string[] NewCouriers()
    {
        return new string[] { "Иванов", "Петров", "Сидоров" };
    }

    private static OrderService NewService(TimeSource time, RandomSource random, OrderRepository repository)
    {
        return new OrderService(time, random, repository, NewCouriers());
    }

    // ----- время: срок доставки считается от времени источника -----

    [Fact]
    public void доставка_курьером_на_600_км_занимает_два_дня_от_времени_источника()
    {
        TimeSource time = new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0));
        OrderService service = NewService(time, new FixedRandomSource(0), new InMemoryOrderRepository());
        Order order = NewOrder();
        order.AssignDelivery(new CourierDelivery(250.0, 600.0));

        DateTime date = service.EstimateDeliveryDate(order);

        Assert.Equal(new DateTime(2026, 1, 12, 12, 0, 0), date);
    }

    [Fact]
    public void доставка_почтой_на_1500_км_занимает_шесть_дней_от_времени_источника()
    {
        TimeSource time = new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0));
        OrderService service = NewService(time, new FixedRandomSource(0), new InMemoryOrderRepository());
        Order order = NewOrder();
        order.AssignDelivery(new PostDelivery(120.0, 1500.0));

        DateTime date = service.EstimateDeliveryDate(order);

        Assert.Equal(new DateTime(2026, 1, 16, 12, 0, 0), date);
    }

    [Fact]
    public void самовывоз_готов_на_следующий_день_от_времени_источника()
    {
        TimeSource time = new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0));
        OrderService service = NewService(time, new FixedRandomSource(0), new InMemoryOrderRepository());
        Order order = NewOrder();
        order.AssignDelivery(new PickupDelivery("Пункт выдачи, ул. Тверская, 7"));

        DateTime date = service.EstimateDeliveryDate(order);

        Assert.Equal(new DateTime(2026, 1, 11, 12, 0, 0), date);
    }

    [Fact]
    public void срок_доставки_переходит_через_границу_месяца()
    {
        TimeSource time = new FixedTimeSource(new DateTime(2026, 1, 31, 12, 0, 0));
        OrderService service = NewService(time, new FixedRandomSource(0), new InMemoryOrderRepository());
        Order order = NewOrder();
        order.AssignDelivery(new CourierDelivery(250.0, 600.0));

        DateTime date = service.EstimateDeliveryDate(order);

        Assert.Equal(new DateTime(2026, 2, 2, 12, 0, 0), date);
    }

    [Fact]
    public void без_назначенной_доставки_дату_рассчитать_нельзя()
    {
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            new InMemoryOrderRepository());

        Assert.Throws<InvalidOperationException>(() => service.EstimateDeliveryDate(NewOrder()));
    }

    [Fact]
    public void расчёт_даты_для_пустого_заказа_отклоняется()
    {
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            new InMemoryOrderRepository());

        Assert.Throws<ArgumentNullException>(() => service.EstimateDeliveryDate(null!));
    }

    // ----- случайность: проверяются все исходы, включая обе границы диапазона -----

    [Theory]
    [InlineData(0, "Иванов")]
    [InlineData(1, "Петров")]
    [InlineData(2, "Сидоров")]
    public void курьер_выбирается_по_числу_от_источника_случайности(int randomValue, string expectedCourier)
    {
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(randomValue),
            new InMemoryOrderRepository());

        string courier = service.AssignCourier();

        Assert.Equal(expectedCourier, courier);
    }

    // первое недопустимое значение сверху (3) и снизу (-1)
    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void номер_курьера_вне_диапазона_отклоняется(int randomValue)
    {
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(randomValue),
            new InMemoryOrderRepository());

        Assert.Throws<InvalidOperationException>(() => service.AssignCourier());
    }

    // ----- хранилище: заказ действительно записывается -----

    [Fact]
    public void зарегистрированный_заказ_попадает_в_хранилище()
    {
        InMemoryOrderRepository repository = new InMemoryOrderRepository();
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            repository);
        Order order = NewOrder();

        service.Register(order);

        Assert.Contains(order, repository.GetAll());
    }

    [Fact]
    public void два_зарегистрированных_заказа_сохраняются_оба()
    {
        InMemoryOrderRepository repository = new InMemoryOrderRepository();
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            repository);

        service.Register(NewOrder());
        service.Register(NewOrder());

        Assert.Equal(2, repository.GetAll().Length);
    }

    [Fact]
    public void регистрация_пустого_заказа_отклоняется()
    {
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            new InMemoryOrderRepository());

        Assert.Throws<ArgumentNullException>(() => service.Register(null!));
    }

    [Fact]
    public void отклонённая_регистрация_ничего_не_записывает_в_хранилище()
    {
        InMemoryOrderRepository repository = new InMemoryOrderRepository();
        OrderService service = NewService(
            new FixedTimeSource(new DateTime(2026, 1, 10, 12, 0, 0)),
            new FixedRandomSource(0),
            repository);

        Record.Exception(() => service.Register(null!));

        Assert.Empty(repository.GetAll());
    }

    // ----- защита конструктора службы -----

    [Fact]
    public void служба_без_источника_времени_не_создаётся()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OrderService(null!, new FixedRandomSource(0), new InMemoryOrderRepository(), NewCouriers()));
    }

    [Fact]
    public void служба_без_источника_случайности_не_создаётся()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OrderService(new FixedTimeSource(DateTime.MinValue), null!, new InMemoryOrderRepository(), NewCouriers()));
    }

    [Fact]
    public void служба_без_хранилища_не_создаётся()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OrderService(new FixedTimeSource(DateTime.MinValue), new FixedRandomSource(0), null!, NewCouriers()));
    }

    [Fact]
    public void служба_с_пустым_списком_курьеров_не_создаётся()
    {
        Assert.Throws<ArgumentException>(() =>
            new OrderService(new FixedTimeSource(DateTime.MinValue), new FixedRandomSource(0), new InMemoryOrderRepository(), new string[0]));
    }
}
