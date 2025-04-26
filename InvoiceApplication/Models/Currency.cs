namespace InvoiceApplication.Models;

/// <summary>
/// Represents a currency with a name, code, and exchange rate relative to a base currency.
/// </summary>
public class Currency
{
    public string Name { get; }
    public string Code { get; }
    public decimal ExchangeRateToBase { get; }

    public Currency(string name, string code, decimal exchangeRateToBase)
    {
        Name = name;
        Code = code;
        ExchangeRateToBase = exchangeRateToBase;
    }

    public override string ToString() => $"{Name} ({Code})";
}
