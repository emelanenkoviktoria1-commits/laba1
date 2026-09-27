namespace OrderLab;


public class OrderQueue
{
    private const int DefaultCapacity = 50;

    private Order?[] orders; // хранилище закрыто
    private int count;       // количество занятых мест закрыто

    public OrderQueue(int capacity = DefaultCapacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Вместимость очереди должна быть положительной");

        orders = new Order?[capacity];
        count = 0;
    }

   
    public void Enqueue(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));
        if (count == orders.Length)
            throw new InvalidOperationException("Очередь заказов переполнена");

        orders[count] = order;
        count++;
    }

   
    public Order? TakeNext()
    {
        if (count == 0)
            return null;

        Order next = orders[0]!;
        for (int i = 1; i < count; i++)
            orders[i - 1] = orders[i];

        count--;
        orders[count] = null;
        return next;
    }

 
    public Order? FindByNumber(int number)
    {
        for (int i = 0; i < count; i++)
        {
            if (orders[i]!.Number == number)
                return orders[i];
        }
        return null;
    }

   
    public double SumPaidOrders()
    {
        double sum = 0;
        for (int i = 0; i < count; i++)
        {
            OrderStatus status = orders[i]!.Status;
            if (status == OrderStatus.Paid || status == OrderStatus.Issued)
                sum += orders[i]!.Sum;
        }
        return sum;
    }

  
    public Dictionary<OrderStatus, int> CountByStatus()
    {
        var result = new Dictionary<OrderStatus, int>();
        foreach (OrderStatus s in Enum.GetValues<OrderStatus>())
            result[s] = 0;

        for (int i = 0; i < count; i++)
            result[orders[i]!.Status]++;

        return result;
    }

   
    public Order[] GetAllOrders()
    {
        var copy = new Order[count];
        Array.Copy(orders, copy, count);
        return copy;
    }

    public int GetCount()
    {
        return count;
    }
}