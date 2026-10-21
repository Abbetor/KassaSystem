using System.Dynamic;

namespace KassaSystem.Domain;

/// <summary>
/// Kassans dag: kvittonummer, dagens kvitton, makuleringar och dagsavslut.
/// </summary>
public class Register
{
    public string CashierName {get; private set;} = "";


    public void SetCashier(string name)
    {
        CashierName = name;
    }
}
