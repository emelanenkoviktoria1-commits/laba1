namespace OrderLab;

// Служба заказов. Три внешних зависимости (время, случайность, хранилище)
// передаются через конструктор, а не создаются внутри: это внедрение зависимости.
// Благодаря этому тест подставляет подставные реализации и получает один и тот же результат при каждом запуске.
public sealed class OrderService
{
    private readonly TimeSource time;
    private readonly RandomSource random;
    private readonly OrderRepository repository;
    private readonly string[] couriers;

    public OrderService(TimeSource time, RandomSource random, OrderRepository repository, string[] couriers)
    {
        if (time is null)
            throw new ArgumentNullException(nameof(time));
        if (random is null)
            throw new ArgumentNullException(nameof(random));
        if (repository is null)
            throw new ArgumentNullException(nameof(repository));
        if (couriers is null)
            throw new ArgumentNullException(nameof(couriers));
        if (couriers.Length == 0)
            throw new ArgumentException("Список курьеров не может быть пустым");

        this.time = time;
        this.random = random;
        this.repository = repository;
        this.couriers = (string[])couriers.Clone();
    }

    // Сохранить заказ в хранилище.
    public void Register(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        repository.Save(order);
    }

    // Назначить курьера: номер выбирает источник случайности.
    public string AssignCourier()
    {
        int index = random.Next(couriers.Length);
        if (index < 0 || index >= couriers.Length)
            throw new InvalidOperationException("Источник случайности вернул номер курьера вне диапазона");

        return couriers[index];
    }

    // Рассчитать дату доставки: текущее время (от источника) плюс срок доставки заказа.
    public DateTime EstimateDeliveryDate(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        Delivery? delivery = order.Delivery;
        if (delivery is null)
            throw new InvalidOperationException("Для заказа не назначена доставка");

        return time.Now().AddDays(delivery.GetTermDays());
    }
}
