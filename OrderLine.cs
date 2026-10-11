namespace OrderLab;

public class OrderLine
{
    private readonly string item;
    private readonly int count;
    private readonly double price;

    internal OrderLine(string item, int count, double price)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Название товара не может быть пустым");
        if (count <= 0)
            throw new ArgumentException("Количество товара должно быть больше нуля");
        if (price <= 0)
            throw new ArgumentException("Цена товара должна быть больше нуля");

        this.item = item;
        this.count = count;
        this.price = price;
    }
    public int Count
    {
        get { return count; }
    }

    public double GetTotal()
    {
        return count * price;
    }

    public override string ToString()
    {
        return $"{item}: {count} шт. x {price:F2} руб. = {GetTotal():F2} руб.";
    }
}



