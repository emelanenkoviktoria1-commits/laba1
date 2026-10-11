namespace OrderLab;

// Абстрактный базовый класс "доставка".
// Общее состояние: стоимость и расстояние. Общий алгоритм: GetTermDays() и ToString().
// Различие наследников: правило расчёта срока (CalculateTermDays) и название способа.
// Доставка неизменяема: все поля readonly, сеттеров нет.
public abstract class Delivery
{
    private readonly double cost;
    private readonly double distanceKm;

    // Конструктор базового класса проверяет инварианты для ВСЕХ наследников,
    // каждый наследник обязан вызвать его через base(...)
    protected Delivery(double cost, double distanceKm)
    {
        if (cost < 0)
            throw new ArgumentException("Стоимость доставки не может быть отрицательной");
        if (distanceKm < 0)
            throw new ArgumentException("Расстояние не может быть отрицательным");

        this.cost = cost;
        this.distanceKm = distanceKm;
    }

    public double Cost
    {
        get { return cost; }
    }

    // доступно только наследникам: им нужно расстояние для расчёта срока
    protected double DistanceKm
    {
        get { return distanceKm; }
    }

    // АБСТРАКТНЫЕ методы: каждый наследник обязан реализовать свои правила
    public abstract string GetMethodName();
    protected abstract int CalculateTermDays();

    // общий алгоритм. метод не virtual, поэтому наследники не могут его переопределить.
    // зачем: любой наследник, в том числе добавленный потом, обязан давать срок не меньше
    // одного дня. проверка лежит в базовом классе и обойти её нельзя.
    public int GetTermDays()
    {
        int days = CalculateTermDays();
        if (days < 1)
            throw new InvalidOperationException("Срок доставки должен быть не меньше одного дня");
        return days;
    }

    // sealed override: ToString() запрещено переопределять в наследниках.
    // Зачем: единый формат описания доставки у всех способов, его не должен менять никто.
    public sealed override string ToString()
    {
        return $"{GetMethodName()}: срок {GetTermDays()} дн., стоимость {cost:F2} руб.";
    }
}
