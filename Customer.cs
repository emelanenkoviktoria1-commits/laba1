namespace OrderLab;

/// все поля закрыты, сеттеров нет,
/// значения задаются один раз в конструкторе и проверяются на непустоту
/// Так как строковые поля в C# сами по себе неизменяемы, а свойства
/// объявлены только с get, объект, полученный через геттер Order.Customer,
/// невозможно изменить извне никаким способом

public sealed class Customer
{
    public string Name { get; }
    public string Phone { get; }
    public string DeliveryAddress { get; }

    public Customer(string name, string phone, string deliveryAddress)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя заказчика не может быть пустым");
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Телефон заказчика не может быть пустым");
        if (string.IsNullOrWhiteSpace(deliveryAddress))
            throw new ArgumentException("Адрес доставки не может быть пустым");

        Name = name;
        Phone = phone;
        DeliveryAddress = deliveryAddress;
    }

    public override string ToString()
    {
        return $"{Name}, тел. {Phone}, адрес доставки: {DeliveryAddress}";
    }
}