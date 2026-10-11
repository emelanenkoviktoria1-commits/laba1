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

    // минимальная сумма, используемая упрощённым конструктором по умолчанию
    private const double DefaultSum = 1.0;

    // все данные заказа закрыты: снаружи их можно только прочитать через геттеры
    private int number;
    private Customer customer;
    private double sum;
    private OrderStatus status;
    private DateTime createdAt;

    private List<OrderLine> lines = new List<OrderLine>();

    private Delivery? delivery;

    private List<DiscountRule> rules = new List<DiscountRule>();


    public int Number
    {
        get { return number; }
    }

    public Customer Customer
    {
        get { return customer; }
    }

    public double Sum
    {
        get { return sum; }
    }

    public OrderStatus Status
    {
        get { return status; }
    }

    public Delivery? Delivery
    {
        get { return delivery; }
    }

    public Order(Customer customer, double sum)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer), "Заказчик должен быть указан");
        if (sum <= 0)
            throw new ArgumentException("Сумма заказа должна быть строго больше нуля");

        orderCounter++;
        this.number = orderCounter;
        this.customer = customer;
        this.sum = sum;
        this.status = OrderStatus.New;
        this.createdAt = DateTime.Now;
    }


    public Order(Customer customer) : this(customer, DefaultSum)
    {
    }

    // заказ с первой позицией позиция создаётся  до увеличения счётчика заказов
    // она вычисляется в аргументе, поэтому при ошибке в позиции номер заказа не тратится.
    public Order(Customer customer, string item, int count, double price)
        : this(customer, new OrderLine(item, count, price))
    {
    }

    private Order(Customer customer, OrderLine firstLine) : this(customer, firstLine.GetTotal())
    {
        lines.Add(firstLine);
    }


    public void Pay()
    {
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Оплатить можно только новый заказ (текущий статус: {status})");
        status = OrderStatus.Paid;
    }


    public void Issue()
    {
        if (status != OrderStatus.Paid)
            throw new InvalidOperationException(
                $"Выдать можно только оплаченный заказ (текущий статус: {status})");
        status = OrderStatus.Issued;
    }

    public void Cancel()
    {
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Отменить можно только неоплаченный заказ (текущий статус: {status})");
        status = OrderStatus.Cancelled;
    }

    // позицию создаём сами это и есть композиции
    public void AddLine(string item, int count, double price)
    {
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Менять состав можно только у нового заказа (текущий статус: {status})");

        OrderLine line = new OrderLine(item, count, price);
        lines.Add(line);
        sum += line.GetTotal();


    }

    // наружу уходят только строки (копии текста), а не сами позиции и не список
    public string[] GetLinesInfo()
    {
        string[] result = new string[lines.Count];
        for (int i = 0; i < lines.Count; i++)
            result[i] = lines[i].ToString();
        return result;
    }

    // общее количество товаров во всех позициях
    public int GetItemsCount()
    {
        int total = 0;
        foreach (OrderLine line in lines)
            total += line.Count;
        return total;
    }
    public void AssignDelivery(Delivery delivery)
    {
        if (delivery is null)
            throw new ArgumentNullException(nameof(delivery), "Доставка должна быть указана");
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Менять доставку можно только у нового заказа (текущий статус: {status})");

        this.delivery = delivery;
    }
    //  правило скидки правило приходит извне его тип заказу неизвестен
    public void AddDiscountRule(DiscountRule rule)
    {
        if (rule is null)
            throw new ArgumentNullException(nameof(rule), "Правило скидки должно быть указано");
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Менять правила можно только у нового заказа (текущий статус: {status})");

        rules.Add(rule);
    }

    // применить все правила одним циклом не проверяя их типы
    // общая скидка ограничена MaxDiscountPercent поэтому инвариант из ЛР1 защищён
    public void ApplyDiscountRules()
    {
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Скидку можно применить только к новому заказу (текущий статус: {status})");

        int itemsCount = GetItemsCount();
        double totalDiscount = 0;

        foreach (DiscountRule rule in rules)
        {
            double discount = rule.CalculateDiscount(sum, itemsCount);
            if (discount < 0)
                throw new InvalidOperationException("Правило скидки вернуло отрицательное значение");
            totalDiscount += discount;
        }

        double maxDiscount = sum * MaxDiscountPercent / 100.0;
        if (totalDiscount > maxDiscount)
            totalDiscount = maxDiscount;

        double newSum = sum - totalDiscount;
        if (newSum <= 0)
            throw new InvalidOperationException(
                "После применения скидки сумма заказа стала бы некорректной (<= 0)");

        sum = newSum;
    }

    // Итог к оплате: сумма товаров плюс доставка (если она назначена).
    public double CalculateTotal()
    {
        if (delivery is null)
            return sum;
        return sum + delivery.Cost;
    }


    public void ApplyDiscount(double percent)
    {

        ApplyDiscount(percent, sum);
    }


    public void ApplyDiscount(double percent, double maxDiscountSum)
    {
        if (status != OrderStatus.New)
            throw new InvalidOperationException(
                $"Скидку можно применить только к новому заказу (текущий статус: {status})");
        if (percent <= 0)
            throw new ArgumentException("Процент скидки должен быть больше нуля");
        if (percent > MaxDiscountPercent)
            throw new ArgumentException(
                $"Скидка не может превышать {MaxDiscountPercent}% (запрошено {percent}%)");
        if (maxDiscountSum <= 0)
            throw new ArgumentException("Максимальная сумма скидки должна быть больше нуля");

        double discount = sum * percent / 100.0;
        if (discount > maxDiscountSum)
            discount = maxDiscountSum;

        double newSum = sum - discount;
        if (newSum <= 0)
            throw new InvalidOperationException(
                "После применения скидки сумма заказа стала бы некорректной (<= 0)");

        sum = newSum;
    }

    public static int GetOrdersCreatedCount()
    {
        return orderCounter;
    }

    public override string ToString()
    {
        return $"Заказ №{number} ({customer.Name}): {sum:F2} руб., статус: {status}, создан: {createdAt:g}";
    }
}