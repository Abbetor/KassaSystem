using KassaSystem.Domain;
using KassaSystem.Storage;

namespace KassaSystem.UI;

/// <summary>
/// Programmets meny och huvudloop.
/// </summary>
public class CashierApp
{
    private readonly List<Product> _Products = new DataStorage().LoadProducts();

    bool running = true;
    private readonly ConsoleUi _Ui = new ConsoleUi();
    private readonly Register _Register = new Register();

    public void Run()
    {
        while (running)
        {
            string choice = _Ui.ShowMenu();

            switch (choice)
            {
                case "1":
                    _Ui.ShowAllProducts(_Products);
                    break;
                case "2":
                    _Ui.ShowProductProperties(_Products);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val");
                    break;
            }
        }
    }
}
