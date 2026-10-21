namespace KassaSystem.Domain;

/// <summary>
/// Avrundning av belopp till öre och hela kronor.
/// </summary>
class Money
{
    public static decimal RoundToOre(decimal SvenskaKronorÖre)
    {
        return Math.Round(SvenskaKronorÖre, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal RoundToKronor(decimal SvenskaKronor)
    {
        return Math.Round(SvenskaKronor, 0, MidpointRounding.AwayFromZero); 
    }
}
