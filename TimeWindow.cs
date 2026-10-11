namespace OrderLab;

// Признак 3: временное окно. Это не иерархия, а значение: "день" и "ночь" это два
// объекта одного класса, а не два класса.
public sealed class TimeWindow
{
    private readonly TimeOnly fromTime;
    private readonly TimeOnly toTime;

    public TimeWindow(TimeOnly fromTime, TimeOnly toTime)
    {
        if (fromTime == toTime)
            throw new ArgumentException("Начало и конец окна не могут совпадать");

        this.fromTime = fromTime;
        this.toTime = toTime;
    }

    public static TimeWindow Day()
    {
        return new TimeWindow(new TimeOnly(9, 0), new TimeOnly(21, 0));
    }

    public static TimeWindow Night()
    {
        return new TimeWindow(new TimeOnly(21, 0), new TimeOnly(6, 0));
    }

    public override string ToString()
    {
        return fromTime.ToString("HH:mm") + "-" + toTime.ToString("HH:mm");
    }
}
