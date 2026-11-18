namespace OrderLab.Tests.Doubles;

// Фейк (fake): рабочее, но упрощённое хранилище, заказы лежат в списке в памяти.
public sealed class InMemoryOrderRepository : OrderRepository
{
    private readonly List<Order> orders = new List<Order>();

    public void Save(Order order)
    {
        orders.Add(order);
    }

    public Order[] GetAll()
    {
        return orders.ToArray();
    }
}
