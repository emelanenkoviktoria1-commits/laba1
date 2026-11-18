namespace OrderLab;

// Источник случайности.
// Контракт: Next(maxExclusive) возвращает целое число от 0 включительно до maxExclusive исключительно.
public interface RandomSource
{
    int Next(int maxExclusive);
}

// Настоящая реализация: генератор случайных чисел .NET.
public sealed class SystemRandomSource : RandomSource
{
    private readonly Random random = new Random();

    public int Next(int maxExclusive)
    {
        return random.Next(maxExclusive);
    }
}
