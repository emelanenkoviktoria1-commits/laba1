namespace OrderLab;

// задание 4. доставка через КОМПОЗИЦИЮ вместо цепочки наследования
// Delivery -> CourierDelivery -> ExpressCourierDelivery -> ExpressCourierNightDelivery
// отношение "имеет", а не "является"
// доставка ИМЕЕТ способ, скорость и временное окно, а не "является курьерской экспресс-ночной".
// признаки независимы: новый способ это один новый класс, остальные не затрагиваются.
public sealed class ComposedDelivery
{
    private readonly DeliveryMethod method;
    private readonly DeliverySpeed speed;
    private readonly TimeWindow window;

    // все три признака приходят извне (агрегация): их можно использовать повторно
    public ComposedDelivery(DeliveryMethod method, DeliverySpeed speed, TimeWindow window)
    {
        if (method is null)
            throw new ArgumentNullException(nameof(method));
        if (speed is null)
            throw new ArgumentNullException(nameof(speed));
        if (window is null)
            throw new ArgumentNullException(nameof(window));

        this.method = method;
        this.speed = speed;
        this.window = window;
    }

    public int CalculateTerm()
    {
        return speed.AdjustDays(method.BaseDays());
    }

    public double CalculateCost()
    {
        return speed.AdjustCost(method.BaseCost());
    }

    public override string ToString()
    {
        return $"Доставка ({method}, {speed}, окно {window}): срок {CalculateTerm()} дн., стоимость {CalculateCost():F2} руб.";
    }
}
