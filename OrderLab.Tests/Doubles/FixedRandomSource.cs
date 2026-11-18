namespace OrderLab.Tests.Doubles;

// Заглушка (stub): всегда возвращает заданное число.
public sealed class FixedRandomSource : RandomSource
{
    private readonly int value;

    public FixedRandomSource(int value)
    {
        this.value = value;
    }

    public int Next(int maxExclusive)
    {
        return value;
    }
}
