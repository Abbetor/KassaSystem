using KassaSystem.Domain;

namespace KassaSystem.UI;

/// <summary>
/// All in- och utmatning mot konsolen.
/// </summary>
public class ConsoleUi
{
    public string AskCashiername()
    {
        while (true)
        {
            Console.Write("Kassörens namn: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("You didnt type anything");
            }
            else if (int.TryParse(name, out _))
            {
                Console.WriteLine("You cant type a number");
            }
            else
            {
                return name;
            }
        }
        
    }

    public string ProductCode()
    {
        while (true)
        {
            Console.Write("Enter the ID code of the product");
            string? code = Console.ReadLine();
        
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("You didnt type anything");
            }
            else if (int.TryParse(code, out _))
            {
                Console.WriteLine("You cant type a number the format is: ABC1");
            }
            else
            {
                return code;
            }
        }
        
    }
    public void ShowProductProperties(List<Product> products)
    {
        bool found = false;
        string code = ProductCode().ToUpper().Trim();
        Console.Clear();
        Console.WriteLine("Lista:\n");
        Console.WriteLine($"{"Kod",-6}{"Namn",-20}{"Pris",10}{"Moms",6}");
        foreach(Product product in products)
        {
            if(product.ProductID == code)
            {
                
                found = true;
                Console.WriteLine($"{product.ProductID,-6}{product.ProductName,-20}{product.ProductPrice,10:F2}{(int)product.VatRate,4} %");
            }
        }

        if(!found)
        {
            Console.WriteLine("Produkten finns inte");
        }

        
    }

    public void ShowAllProducts(List<Product> products)
    {
        Console.WriteLine($"{"Kod",-6}{"Namn",-20}{"Pris",10}{"Moms",6}");

        foreach (Product product in products)
        {
            Console.WriteLine($"{product.ProductID,-6}{product.ProductName,-20}{product.ProductPrice,10:F2}{(int)product.VatRate,4} %");
        }
    }

    public string ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("1. Visa alla produkter");
        Console.WriteLine("2. Sök produkt");
        Console.WriteLine("0. Avsluta");
        Console.Write("Välj: ");

        string? choice = Console.ReadLine();
        return choice ?? "";
    }



    
}
