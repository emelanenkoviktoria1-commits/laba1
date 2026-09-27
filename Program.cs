namespace OrderLab;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        //подготовка данных 

        var customer1 = new Customer("Иванов Пётр", "+7-900-111-22-33", "г. Москва, ул. Ленина, 1");
        var customer2 = new Customer("Сидорова Анна", "+7-900-444-55-66", "г. Казань, ул. Баумана, 5");
        var customer3 = new Customer("Кузнецов Олег", "+7-900-777-88-99", "г. Краснодар, ул. Мира, 10");

        var order1 = new Order(customer1, 5000);
        var order2 = new Order(customer2, 12000);
        var order3 = new Order(customer3, 800);
        var order4 = new Order(customer1); 

        var queue = new OrderQueue();
        queue.Enqueue(order1);
        queue.Enqueue(order2);
        queue.Enqueue(order3);
        queue.Enqueue(order4);

        // корректные операции 

        Console.WriteLine("=== Корректные операции ===");

        order1.Pay();
        order1.Issue();
        Console.WriteLine(order1);

        order2.ApplyDiscount(10); // короткая версия перегрузки
        order2.Pay();
        Console.WriteLine(order2);

        order3.ApplyDiscount(20, 100); // полная версия
        order3.Cancel(); // заказ ещё не оплачен — отмена разрешена
        Console.WriteLine(order3);

        Console.WriteLine($"Всего создано заказов: {Order.GetOrdersCreatedCount()}");
        Console.WriteLine($"Оплачено на сумму: {queue.SumPaidOrders()} руб.");

        // попытка 1: создать заказ на сумму 0 

        Console.WriteLine();
        Console.WriteLine("=== Демонстрация защиты состояния ===");
        Console.WriteLine("Попытка: создать заказ с суммой 0");
        try
        {
            var badOrder = new Order(customer1, 0);
            Console.WriteLine("  Ошибка теста: недопустимый заказ был создан!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }

        // попытка 2: перевести заказ из "выдан" в "новый"

        Console.WriteLine("Попытка: отменить уже выданный заказ (order1 в статусе Issued)");
        Console.WriteLine($"  Состояние до попытки: {order1}");
        try
        {
            order1.Cancel();
            Console.WriteLine("  Ошибка теста: статус изменился, хотя не должен был!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Состояние после попытки не изменилось: {order1}");

        // попытка 3: отменить оплаченный заказ 

        Console.WriteLine("Попытка: отменить оплаченный заказ (order2 в статусе Paid)");
        Console.WriteLine($"  Состояние до попытки: {order2}");
        try
        {
            order2.Cancel();
            Console.WriteLine("  Ошибка теста: статус изменился, хотя не должен был!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Состояние после попытки не изменилось: {order2}");

        // попытка 4: скидка больше MAX_DISCOUNT 

        var order5 = new Order(customer2, 3000);
        queue.Enqueue(order5);
        double tooBigDiscount = Order.MaxDiscountPercent + 20;

        Console.WriteLine($"Попытка: применить скидку {tooBigDiscount}% ");
        Console.WriteLine($"  Состояние до попытки: {order5}");
        try
        {
            order5.ApplyDiscount(tooBigDiscount);
            Console.WriteLine("  Ошибка теста: сумма изменилась, хотя не должна была!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  Отбито: {ex.Message}");
        }
        Console.WriteLine($"  Состояние после попытки не изменилось: {order5}");


        Console.WriteLine();
        Console.WriteLine("=== Защита вложенного объекта Customer ===");
        Customer obtained = order1.Customer;
        Console.WriteLine($"Получили заказчика через геттер: {obtained}");
        Console.WriteLine("У Customer нет ни одного сеттера — изменить телефон через " +
                           "полученный объект невозможно даже теоретически.");

        // операции очереди заказов

        Console.WriteLine();
        Console.WriteLine("=== Операции очереди заказов ===");

        var statuses = queue.CountByStatus();
        foreach (var pair in statuses)
            Console.WriteLine($"{pair.Key}: {pair.Value}");

        Order? found = queue.FindByNumber(order2.Number);
        Console.WriteLine($"Найден по номеру {order2.Number}: {found}");

        Order? notFound = queue.FindByNumber(9999);
        if (notFound is null)
            Console.WriteLine("Поиск несуществующего номера 9999 -> null (без падения программы)");
        else
            Console.WriteLine($"Поиск несуществующего номера 9999 -> {notFound}");

        Order? next = queue.TakeNext();
        Console.WriteLine($"Взяли следующий из очереди: {next}");

        Order[] snapshot = queue.GetAllOrders(); // наружу отдаётся копия, не сам массив
        Console.WriteLine($"Снимок очереди (копия), заказов: {snapshot.Length}");
    }
}

/*
 * Сводка "поле — правило — метод, где правило проверяется":
 *
 * Order.Sum         — сумма заказа строго больше нуля
 *                      — проверяется в конструкторе Order(Customer, double)
 *                        и в Order.ApplyDiscount(double, double) (newSum <= 0 запрещён).
 *
 * Order.Status      — статус меняется только по цепочке New -> Paid -> Issued,
 *                      откат назад невозможен
 *                      — проверяется в Order.Pay() (только из New) и
 *                        Order.Issue() (только из Paid); у класса нет ни
 *                        одного метода, позволяющего установить статус
 *                        напрямую или откатить его назад.
 *
 * Order.Status      — отменить можно только неоплаченный (New) заказ
 *                      — проверяется в Order.Cancel().
 *
 * Order.Number      — номер задаётся один раз при создании и не меняется
 *                      — свойство Number объявлено только с get,
 *                        присваивается один раз в конструкторе Order.
 *
 * Order (скидка)    — процент скидки не может превышать MaxDiscountPercent
 *                      — проверяется в Order.ApplyDiscount(double, double).
 *
 * Customer.Name/
 * Phone/
 * DeliveryAddress   — поля заказчика не могут быть пустыми
 *                      — проверяется в конструкторе Customer(string, string, string).
 *
 * Customer (объект) — данные заказчика нельзя изменить через объект,
 *                      полученный геттером Order.Customer
 *                      — обеспечивается тем, что Customer — sealed класс
 *                        без единого сеттера (все свойства только get).
 *
 * OrderQueue.orders — внутренний массив очереди не отдаётся наружу
 *                      — GetAllOrders() возвращает Array.Copy, а не сам массив.
 */