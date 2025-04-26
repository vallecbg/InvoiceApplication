namespace InvoiceApplication.Models;

/// <summary>
/// Represents an item in an invoice.
/// </summary>
public class InvoiceItem<TCurrency> where TCurrency : Currency
{
    public string Name { get; }
    public SingleCurrencyAmount<TCurrency> Price { get; }
    public int Quantity { get; }

    public InvoiceItem(string name, SingleCurrencyAmount<TCurrency> price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    /// <summary>
    /// Gets the total amount for this item (Price * Quantity).
    /// </summary>
    public decimal Total => Price.Amount * Quantity;

    /// <summary>
    /// Gets the total amount for this item as a SingleCurrencyAmount in the original currency.
    /// </summary>
    public SingleCurrencyAmount<TCurrency> GetTotal()
    {
        return new SingleCurrencyAmount<TCurrency>(Price.Amount * Quantity, Price.Currency);
    }

    /// <summary>
    /// Gets the price of the item in the selected currency.
    /// </summary>
    public SingleCurrencyAmount<TTargetCurrency> GetPriceInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        return Price.ConvertTo(targetCurrency);
    }

    /// <summary>
    /// Gets the total amount in the selected currency.
    /// </summary>
    public SingleCurrencyAmount<TTargetCurrency> GetTotalInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        var totalInOriginalCurrency = GetTotal();
        return totalInOriginalCurrency.ConvertTo(targetCurrency);
    }

    public override string ToString()
    {
        return $"{Name} - {Quantity} x {Price.Amount:F2} {Price.Currency.Code}";
    }
}
