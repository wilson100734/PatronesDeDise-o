using System;
using System.Collections.Generic;
using System.Linq;

// Patrón Observer: Define una dependencia uno a muchos entre objetos para que cuando un objeto cambie su estado,
// todos sus dependientes sean notificados y actualizados automáticamente.
// En este caso, los usuarios pueden suscribirse para recibir notificaciones cuando se agreguen nuevos productos.
public interface IObserver
{
    void Update(string productName);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string productName);
}

public class ShopSubject : ISubject
{
    private List<IObserver> observers;
    private List<string> products;

    public ShopSubject()
    {
        observers = new List<IObserver>();
        products = new List<string>();
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(string productName)
    {
        foreach (var observer in observers)
        {
            observer.Update(productName);
        }
    }

    public void AddProduct(string productName)
    {
        products.Add(productName);
        Notify(productName);
    }
}

public class UserObserver : IObserver
{
    private string name;

    public UserObserver(string name)
    {
        this.name = name;
    }

    public void Update(string productName)
    {
        Console.WriteLine($"Hola {name}, ¡Nuevo producto agregado! Nombre del producto: {productName}");
    }
}

// Patrón Strategy: Define una familia de algoritmos, encapsula cada uno de ellos y los hace intercambiables.
// En este caso, representamos diferentes estrategias de cálculo de precio para los productos.
public interface IPriceCalculationStrategy
{
    double CalculatePrice(double basePrice, int quantity);
}

// Implementación de una estrategia de cálculo de precio simple: precio base * cantidad
public class SimplePriceCalculationStrategy : IPriceCalculationStrategy
{
    public double CalculatePrice(double basePrice, int quantity)
    {
        return basePrice * quantity;
    }
}

// Implementación de una estrategia de cálculo de precio con descuento por cantidad
public class DiscountPriceCalculationStrategy : IPriceCalculationStrategy
{
    public double CalculatePrice(double basePrice, int quantity)
    {
        const double discountPercentage = 0.9; // 10% de descuento
        return basePrice * quantity * discountPercentage;
    }
}

// Patrón Singleton: Garantiza que una clase tenga solo una instancia y proporciona un punto de acceso global a ella.
// En este caso, usamos el patrón Singleton para manejar el carrito de compras.
public class ShoppingCart
{
    private static ShoppingCart instance;
    private List<Product> products;

    private ShoppingCart()
    {
        products = new List<Product>();
    }

    public static ShoppingCart GetInstance()
    {
        if (instance == null)
        {
            instance = new ShoppingCart();
        }
        return instance;
    }

    public void AddProduct(Product product, int quantity)
    {
        product.Quantity = quantity;
        products.Add(product);
        Console.WriteLine($"{quantity} {product.Name}(s) agregado(s) al carrito.");
    }

    public void RemoveProduct(Product product)
    {
        products.Remove(product);
        Console.WriteLine($"{product.Name} eliminado del carrito.");
    }

    public void ViewCart()
    {
        if (products.Any())
        {
            Console.WriteLine("Contenido del carrito:");
            double total = 0;
            foreach (var product in products)
            {
                Console.WriteLine($"- {product.Name} (Cantidad: {product.Quantity}) - Precio: {product.Price}");
                total += product.Price;
            }
            Console.WriteLine($"Total del carrito: {total}");
        }
        else
        {
            Console.WriteLine("El carrito está vacío.");
        }
    }

    public void FinalizeOrder(string address, string phoneNumber, string paymentMethod)
    {
        // Verificar si hay productos en el carrito antes de finalizar el pedido
        if (products.Any())
        {
            // Aquí puedes implementar la lógica para finalizar el pedido, por ejemplo, enviar los detalles del pedido a una base de datos o servicio externo.
            Console.WriteLine($"Dirección de envío: {address}");
            Console.WriteLine($"Número de teléfono: {phoneNumber}");
            Console.WriteLine($"Forma de pago: {paymentMethod}");
            Console.WriteLine("¡Tu pedido ha sido finalizado!");
            products.Clear(); // Limpiar el carrito después de finalizar el pedido
        }
        else
        {
            Console.WriteLine("No puedes finalizar el pedido porque el carrito está vacío.");
        }
    }

    public void ClearCart()
    {
        products.Clear();
        Console.WriteLine("El carrito ha sido vaciado.");
    }

    public void ViewAvailableProducts(List<Product> availableProducts)
    {
        Console.WriteLine("Productos disponibles:");
        foreach (var product in availableProducts)
        {
            Console.WriteLine($"- {product.Name} - Precio: {product.BasePrice}");
        }
    }
}

// Clase que representa un producto en la tienda en línea
public class Product
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double BasePrice { get; set; }
    public int Quantity { get; set; }
    public double Price { get; private set; }

    private IPriceCalculationStrategy priceStrategy;

    public Product(string name, string description, double basePrice, IPriceCalculationStrategy strategy)
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        priceStrategy = strategy;
        CalculatePrice(1); // Calcular precio inicial para una cantidad de 1
    }

    public void CalculatePrice(int quantity)
    {
        Quantity = quantity;
        Price = priceStrategy.CalculatePrice(BasePrice, Quantity);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var simplePriceStrategy = new SimplePriceCalculationStrategy();
        var discountPriceStrategy = new DiscountPriceCalculationStrategy();

        var product1 = new Product("Producto 1", "Descripción del Producto 1", 10.0, simplePriceStrategy);
        var product2 = new Product("Producto 2", "Descripción del Producto 2", 20.0, discountPriceStrategy);

        var availableProducts = new List<Product> { product1, product2 };

        var shoppingCart = ShoppingCart.GetInstance();

        var shopSubject = new ShopSubject();

        var user1 = new UserObserver("Usuario1");
        var user2 = new UserObserver("Usuario2");

        shopSubject.Attach(user1);
        shopSubject.Attach(user2);

        shopSubject.AddProduct("Nuevo Producto");

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n1. Agregar producto al carrito");
            Console.WriteLine("2. Ver carrito");
            Console.WriteLine("3. Eliminar producto del carrito");
            Console.WriteLine("4. Vaciar carrito");
            Console.WriteLine("5. Ver productos disponibles");
            Console.WriteLine("6. Finalizar pedido");
            Console.WriteLine("7. Salir");
            Console.Write("\nSeleccione una opción: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("\nIngrese el nombre del producto: ");
                    string productName = Console.ReadLine();
                    Console.Write("Ingrese la cantidad de productos: ");
                    int quantity;
                    if (int.TryParse(Console.ReadLine(), out quantity))
                    {
                        var product = availableProducts.FirstOrDefault(p => p.Name == productName);
                        if (product != null)
                        {
                            product.CalculatePrice(quantity);
                            shoppingCart.AddProduct(product, quantity);
                        }
                        else
                        {
                            Console.WriteLine("Producto no encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cantidad no válida.");
                    }
                    break;
                case "2":
                    shoppingCart.ViewCart();
                    break;
                case "3":
                    // Implementa lógica para eliminar un producto específico del carrito
                    Console.WriteLine("Función no implementada.");
                    break;
                case "4":
                    shoppingCart.ClearCart();
                    break;
                case "5":
                    shoppingCart.ViewAvailableProducts(availableProducts);
                    break;
                case "6":
                    Console.WriteLine("\nFinalizar Pedido:");
                    Console.Write("Ingrese la dirección de envío: ");
                    string address = Console.ReadLine();
                    Console.Write("Ingrese el número de teléfono: ");
                    string phoneNumber = Console.ReadLine();
                    Console.Write("Ingrese la forma de pago: ");
                    string paymentMethod = Console.ReadLine();
                    shoppingCart.FinalizeOrder(address, phoneNumber, paymentMethod);
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Por favor, seleccione una opción válida.");
                    break;
            }
        }
    }
}

