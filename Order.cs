namespace OrderLab;

///инварианты:
///  сумма заказа строго больше нуля всегда (в момент создания и после скидки)
///  статус меняется только по цепочке new -> paid -> issued, откат назад невозможен
///  отменить можно только заказ в статусе new 
///  номер заказа задаётся один раз при создании и не может быть изменён
/// 
public class Order
{
    //  счётчик заказов
    private static int orderCounter = 0;

    //  максимально допустимый процент скидки
    public const double MaxDiscountPercent = 30.0;

    // vинимальная сумма, используемая упрощённым конструктором по умолчанию
    private const double DefaultSum = 1.0;

    public int Number { get; }
    public Customer Customer { get; }
    public double Sum { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; }


    public Order(Customer customer, double sum)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer), "Заказчик должен быть указан");
        if (sum <= 0)
            throw new ArgumentException("Сумма заказа должна быть строго больше нуля");

        orderCounter++;
        Number = orderCounter;
        Customer = customer;
        Sum = sum;
        Status = OrderStatus.New;
        CreatedAt = DateTime.Now;
    }

   
    public Order(Customer customer) : this(customer, DefaultSum)
    {
    }

    public void Pay()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Оплатить можно только новый заказ (текущий статус: {Status})");
        Status = OrderStatus.Paid;
    }

    
    public void Issue()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException(
                $"Выдать можно только оплаченный заказ (текущий статус: {Status})");
        Status = OrderStatus.Issued;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Отменить можно только неоплаченный заказ (текущий статус: {Status})");
        Status = OrderStatus.Cancelled;
    }

    
    public void ApplyDiscount(double percent)
    {
        
        ApplyDiscount(percent, Sum);
    }

    
    public void ApplyDiscount(double percent, double maxDiscountSum)
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Скидку можно применить только к новому заказу (текущий статус: {Status})");
        if (percent <= 0)
            throw new ArgumentException("Процент скидки должен быть больше нуля");
        if (percent > MaxDiscountPercent)
            throw new ArgumentException(
                $"Скидка не может превышать {MaxDiscountPercent}% (запрошено {percent}%)");
        if (maxDiscountSum <= 0)
            throw new ArgumentException("Максимальная сумма скидки должна быть больше нуля");

        double discount = Sum * percent / 100.0;
        if (discount > maxDiscountSum)
            discount = maxDiscountSum;

        double newSum = Sum - discount;
        if (newSum <= 0)
            throw new InvalidOperationException(
                "После применения скидки сумма заказа стала бы некорректной (<= 0)");

        Sum = newSum;
    }

    public static int GetOrdersCreatedCount()
    {
        return orderCounter;
    }

    public override string ToString()
    {
        return $"Заказ №{Number} ({Customer.Name}): {Sum:F2} руб., статус: {Status}, создан: {CreatedAt:g}";
    }
}