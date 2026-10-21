namespace KassaSystem.Storage;

using System.Globalization;

using KassaSystem.Domain;


/// <summary>
/// Läser och sparar sortiment, kvitton, räknare och Z-rapporter till och från fil.
/// </summary>
public class DataStorage
{
    public List<Product> LoadProducts()
    {
        var products = new List<Product>();
        string[] lines = File.ReadAllLines("Products.txt");

        foreach(string line in lines)
        {
            string[] parts = line.Split(';');
            var product = new Product(
                parts[0],
                parts[1],
                Enum.Parse<VatRate>(parts[3]),
                decimal.Parse(parts[2], CultureInfo.InvariantCulture));
            products.Add(product);
            
        }
        return products;
    }
}
