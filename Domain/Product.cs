namespace KassaSystem.Domain;

/// <summary>
/// En produkt i sortimentet: kod, namn, styckpris och momssats.
/// </summary>
public class Product
{
    public string ProductID {get; private set;}
    public string ProductName {get; private set;}
    public VatRate VatRate { get; private set; }
    public decimal ProductPrice { get; private set; }


    public Product(string id, string name, VatRate vatRate , decimal price)
    {
        ProductID = id;
        ProductName = name;
        VatRate = vatRate;
        ProductPrice = price;
    }
    

}
