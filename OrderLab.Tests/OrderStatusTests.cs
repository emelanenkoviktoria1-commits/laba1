using Xunit;

namespace OrderLab.Tests;

// Инварианты: статус меняется только по цепочке New -> Paid -> Issued;
// отменить можно только неоплаченный заказ.
public class OrderStatusTests
{
    private static Order NewOrder()
    {
        Customer customer = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        return new Order(customer, 1000.0);
    }

    private static Order PaidOrder()
    {
        Order order = NewOrder();
        order.Pay();
        return order;
    }

    private static Order IssuedOrder()
    {
        Order order = PaidOrder();
        order.Issue();
        return order;
    }

    private static Order CancelledOrder()
    {
        Order order = NewOrder();
        order.Cancel();
        return order;
    }

    // ----- разрешённые переходы -----

    [Fact]
    public void новый_заказ_можно_оплатить()
    {
        Order order = NewOrder();

        order.Pay();

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void оплаченный_заказ_можно_выдать()
    {
        Order order = PaidOrder();

        order.Issue();

        Assert.Equal(OrderStatus.Issued, order.Status);
    }

    [Fact]
    public void неоплаченный_заказ_можно_отменить()
    {
        Order order = NewOrder();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // ----- запрещённые переходы вперёд: нельзя пропускать шаги цепочки -----

    [Fact]
    public void новый_заказ_нельзя_выдать_минуя_оплату()
    {
        Order order = NewOrder();

        Assert.Throws<InvalidOperationException>(() => order.Issue());
    }

    [Fact]
    public void оплаченный_заказ_нельзя_оплатить_повторно()
    {
        Order order = PaidOrder();

        Assert.Throws<InvalidOperationException>(() => order.Pay());
    }

    [Fact]
    public void выданный_заказ_нельзя_выдать_повторно()
    {
        Order order = IssuedOrder();

        Assert.Throws<InvalidOperationException>(() => order.Issue());
    }

    // ----- запрещённые переходы назад: откат невозможен -----

    [Fact]
    public void выданный_заказ_нельзя_оплатить_откат_назад_запрещён()
    {
        Order order = IssuedOrder();

        Assert.Throws<InvalidOperationException>(() => order.Pay());
    }

    // ----- отмена только неоплаченного -----

    [Fact]
    public void оплаченный_заказ_нельзя_отменить()
    {
        Order order = PaidOrder();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void выданный_заказ_нельзя_отменить()
    {
        Order order = IssuedOrder();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void отменённый_заказ_нельзя_отменить_повторно()
    {
        Order order = CancelledOrder();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void отменённый_заказ_нельзя_оплатить()
    {
        Order order = CancelledOrder();

        Assert.Throws<InvalidOperationException>(() => order.Pay());
    }

    [Fact]
    public void отменённый_заказ_нельзя_выдать()
    {
        Order order = CancelledOrder();

        Assert.Throws<InvalidOperationException>(() => order.Issue());
    }

    // ----- отклонённая операция не должна менять состояние -----

    [Fact]
    public void после_отклонённой_отмены_оплаченный_заказ_остаётся_оплаченным()
    {
        Order order = PaidOrder();

        Record.Exception(() => order.Cancel());

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void после_отклонённой_выдачи_новый_заказ_остаётся_новым()
    {
        Order order = NewOrder();

        Record.Exception(() => order.Issue());

        Assert.Equal(OrderStatus.New, order.Status);
    }
}
