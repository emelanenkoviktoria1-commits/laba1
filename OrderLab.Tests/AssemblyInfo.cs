using Xunit;

// Счётчик заказов в Order статический, то есть общий для всех тестов.
// Чтобы тесты не влияли друг на друга, они выполняются по одному, а не параллельно.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
