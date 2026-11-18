namespace OrderLab;

// Хранилище заказов. Служба работает с этим интерфейсом и не знает,
// где именно лежат заказы: в очереди, в памяти теста или в базе данных.
public interface OrderRepository
{
    void Save(Order order);
    Order[] GetAll();
}

// Настоящая реализация: хранилищем служит очередь заказов из ЛР1.
public sealed class QueueOrderRepository : OrderRepository
{
    private readonly OrderQueue queue;

    public QueueOrderRepository(OrderQueue queue)
    {
        if (queue is null)
            throw new ArgumentNullException(nameof(queue));

        this.queue = queue;
    }

    public void Save(Order order)
    {
        queue.Enqueue(order);
    }

    public Order[] GetAll()
    {
        return queue.GetAllOrders();
    }
}
