namespace InvoiceApplication.Models;

/// <summary>
/// Represents an amount in a single currency.
/// </summary>
public class SingleCurrencyAmount<TCurrency> where TCurrency : Currency
{
    public decimal Amount { get; }
    public TCurrency Currency { get; }

    // Празен конструктор за EF
    private SingleCurrencyAmount() { }
    public SingleCurrencyAmount(decimal amount, TCurrency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency.Code}";

    // Explicit conversion to another currency
    public SingleCurrencyAmount<TTargetCurrency> ConvertTo<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        if (Currency == targetCurrency)
            return new SingleCurrencyAmount<TTargetCurrency>(Amount, targetCurrency);

        decimal convertedAmount = Amount * (targetCurrency.ExchangeRateToBase / Currency.ExchangeRateToBase);
        return new SingleCurrencyAmount<TTargetCurrency>(convertedAmount, targetCurrency);
    }

    // Prevent operations between different currencies at compile-time
    public static SingleCurrencyAmount<TCurrency> operator +(SingleCurrencyAmount<TCurrency> a, SingleCurrencyAmount<TCurrency> b)
    {
        return new SingleCurrencyAmount<TCurrency>(a.Amount + b.Amount, a.Currency);
    }

    public static SingleCurrencyAmount<TCurrency> operator -(SingleCurrencyAmount<TCurrency> a, SingleCurrencyAmount<TCurrency> b)
    {
        return new SingleCurrencyAmount<TCurrency>(a.Amount - b.Amount, a.Currency);
    }
}