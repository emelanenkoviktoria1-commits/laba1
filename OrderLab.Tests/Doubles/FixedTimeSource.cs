namespace OrderLab.Tests.Doubles;

// Заглушка (stub): всегда возвращает заданное время.
public sealed class FixedTimeSource : TimeSource
{
    private readonly DateTime now;

    public FixedTimeSource(DateTime now)
    {
        this.now = now;
    }

    public DateTime Now()
    {
        return now;
    }
}
