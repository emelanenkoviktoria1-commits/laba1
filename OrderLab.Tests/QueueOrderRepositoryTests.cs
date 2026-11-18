using Xunit;

namespace OrderLab.Tests;

// Настоящая реализация хранилища на основе очереди заказов из ЛР1.
public class QueueOrderRepositoryTests
{
    private static Order NewOrder()
    {
        Customer customer = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        return new Order(customer, 1000.0);
    }

    [Fact]
    public void сохранённый_заказ_виден_в_списке_заказов()
    {
        QueueOrderRepository repository = new QueueOrderRepository(new OrderQueue());
        Order order = NewOrder();

        repository.Save(order);

        Assert.Contains(order, repository.GetAll());
    }

    [Fact]
    public void заказы_возвращаются_в_порядке_сохранения()
    {
        QueueOrderRepository repository = new QueueOrderRepository(new OrderQueue());
        Order first = NewOrder();
        Order second = NewOrder();
        repository.Save(first);
        repository.Save(second);

        Order[] all = repository.GetAll();

        Assert.Equal(new Order[] { first, second }, all);
    }

    [Fact]
    public void в_переполненную_очередь_сохранить_нельзя()
    {
        QueueOrderRepository repository = new QueueOrderRepository(new OrderQueue(1));
        repository.Save(NewOrder());

        Assert.Throws<InvalidOperationException>(() => repository.Save(NewOrder()));
    }

    [Fact]
    public void хранилище_без_очереди_не_создаётся()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueOrderRepository(null!));
    }
}
