namespace OrderLab;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;


        var customer1 = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        var customer2 = new Customer("Сидорова Анна", "+7-900-444-55-66", "г. Казань, ул. Баумана, 5");

        // разнородные доставки лежат в массиве базового типа
        Delivery[] deliveries =
        {
            new CourierDelivery(250, 600),
            new PickupDelivery("Пункт выдачи, ул. Тверская, 7"),
            new PostDelivery(120, 1500)
        };

        // разные правила скидок лежат в массиве типа интерфейса
        DiscountRule[] discountRules =
        {
            new NoDiscount(),
            new PercentDiscount(10),
            new VolumeDiscount(5, 15)
        };

        DiscountRule hugeDiscount = new PercentDiscount(50);

        // задание 4: доставка из независимых признаков
        ComposedDelivery[] composedDeliveries =
        {
            new ComposedDelivery(new CourierMethod(), new StandardSpeed(), TimeWindow.Day()),
            new ComposedDelivery(new CourierMethod(), new ExpressSpeed(), TimeWindow.Night()),
            new ComposedDelivery(new PostMethod(), new StandardSpeed(), TimeWindow.Day())
        };

       
  

        Console.WriteLine("=== Задание 1. Композиция и ассоциация ===");

        var order = new Order(customer1, "Ноутбук", 1, 60000);
        order.AddLine("Мышь", 2, 1500);      // позиции создаёт сам заказ
        order.AddLine("Коврик", 3, 500);

        Console.WriteLine(order);
        foreach (string line in order.GetLinesInfo())
            Console.WriteLine("  " + line);
        Console.WriteLine($"Всего товаров: {order.GetItemsCount()}");
        Console.WriteLine($"Заказчик (ассоциация, живёт независимо): {order.Customer}");


        Console.WriteLine();
        Console.WriteLine("=== Задание 2. Иерархия доставок ===");

        // один цикл, без проверок типа: каждый объект считает срок по-своему
        foreach (Delivery d in deliveries)
            Console.WriteLine(d);

        order.AssignDelivery(deliveries[0]);
        Console.WriteLine($"Итого к оплате с доставкой: {order.CalculateTotal():F2} руб.");

        Console.WriteLine();
        Console.WriteLine("=== Задание 3. Правила скидок ===");

        // один цикл по разным правилам, без проверок типа
        foreach (DiscountRule rule in discountRules)
        {
            double discount = rule.CalculateDiscount(order.Sum, order.GetItemsCount());
            Console.WriteLine($"  {rule}: {discount:F2} руб.");
        }

        // заказ получает все правила и применяет их, не зная, какие они
        foreach (DiscountRule rule in discountRules)
            order.AddDiscountRule(rule);

        order.ApplyDiscountRules();
        Console.WriteLine($"После применения всех правил: {order}");
        Console.WriteLine($"Итого к оплате с доставкой: {order.CalculateTotal():F2} руб.");

      
        Console.WriteLine();
        Console.WriteLine("=== Задание 4. Доставка из способа, скорости и окна ===");

        foreach (ComposedDelivery d in composedDeliveries)
            Console.WriteLine(d);

        Console.WriteLine("Иерархия при 2 признаках (способ x скорость): 3 x 2 = 6 классов, композиция: 3 + 2 = 5 объектов");
        Console.WriteLine("Иерархия при 3 признаках (способ x скорость x окно): 3 x 2 x 2 = 12 классов, композиция: 3 + 2 + 2 = 7 объектов");

      

        Console.WriteLine();
        Console.WriteLine("=== Инварианты ЛР1 после добавления связей ===");

        // Общая скидка ограничена MaxDiscountPercent
        var order2 = new Order(customer2, "Телефон", 2, 20000);
        order2.AddDiscountRule(hugeDiscount);
        Console.WriteLine($"До скидки в 50%: {order2.Sum:F2} руб.");
        order2.ApplyDiscountRules();
        Console.WriteLine($"После (скидка урезана до {Order.MaxDiscountPercent}%): {order2.Sum:F2} руб.");

        // Попытка 1: позиция с нулевым количеством. Номер заказа не должен тратиться.
        Console.WriteLine("Попытка: создать заказ с позицией на 0 штук");
        int createdBefore = Order.GetOrdersCreatedCount();
        try
        {
            var bad = new Order(customer1, "Чехол", 0, 500);
            Console.WriteLine("  Ошибка теста: недопустимый заказ был создан!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Создано заказов до: {createdBefore}, после: {Order.GetOrdersCreatedCount()}");

        // Попытка 2: добавить позицию в оплаченный заказ
        order2.Pay();
        Console.WriteLine("Попытка: добавить позицию в оплаченный заказ");
        try
        {
            order2.AddLine("Чехол", 1, 500);
            Console.WriteLine("  Ошибка теста: состав оплаченного заказа изменился!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Состояние не изменилось: {order2}");

        // Попытка 3: отменить оплаченный заказ
        Console.WriteLine("Попытка: отменить оплаченный заказ");
        try
        {
            order2.Cancel();
            Console.WriteLine("  Ошибка теста: оплаченный заказ отменён!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Состояние не изменилось: {order2}");
    }
}
