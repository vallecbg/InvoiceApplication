namespace InvoiceApplication.Models;

/// <summary>
/// Represents an item in an invoice.
/// </summary>
public class InvoiceItem<TCurrency> where TCurrency : Currency
{
    public string Name { get; }
    public SingleCurrencyAmount<TCurrency> Price { get; }
    public int Quantity { get; }
    public decimal VATPercentage { get; }
    public int DiscountQuantityThreshold { get; } // Минимално количество за отстъпка
    public decimal DiscountPercentage { get; } // Процент на отстъпка
    
    // Параметърless конструктор за EF Core
    private InvoiceItem() { }

    public InvoiceItem(string name, SingleCurrencyAmount<TCurrency> price, int quantity, decimal vatPercentage = 20, int discountQuantityThreshold = 3, decimal discountPercentage = 5)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        VATPercentage = vatPercentage;
        DiscountQuantityThreshold = discountQuantityThreshold;
        DiscountPercentage = discountPercentage;
    }

    /// <summary>
    /// Gets the total amount for this item (Price * Quantity).
    /// </summary>
    public decimal Total => Price.Amount * Quantity - DiscountAmount;

    /// <summary>
    /// Gets the discount amount for this item.
    /// </summary>
    public decimal DiscountAmount => Quantity >= DiscountQuantityThreshold ? (Price.Amount * Quantity * (DiscountPercentage / 100)) : 0;

    /// <summary>
    /// Gets the discount amount in the selected currency.
    /// </summary>
    public SingleCurrencyAmount<TTargetCurrency> GetDiscountAmountInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        var discountInOriginalCurrency = new SingleCurrencyAmount<TCurrency>(DiscountAmount, Price.Currency);
        return discountInOriginalCurrency.ConvertTo(targetCurrency);
    }

    /// <summary>
    /// Gets the VAT amount for this item.
    /// </summary>
    public decimal VATAmount => Total * (VATPercentage / 100);

    /// <summary>
    /// Gets the total amount including VAT.
    /// </summary>
    public decimal TotalWithVAT => Total + VATAmount;

    /// <summary>
    /// Gets the VAT amount in the selected currency.
    /// </summary>
    public SingleCurrencyAmount<TTargetCurrency> GetVATAmountInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        var vatInOriginalCurrency = new SingleCurrencyAmount<TCurrency>(VATAmount, Price.Currency);
        return vatInOriginalCurrency.ConvertTo(targetCurrency);
    }

    /// <summary>
    /// Gets the total amount including VAT in the selected currency.
    /// </summary>
    public SingleCurrencyAmount<TTargetCurrency> GetTotalWithVATInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
        where TTargetCurrency : Currency
    {
        var totalWithVATInOriginalCurrency = new SingleCurrencyAmount<TCurrency>(TotalWithVAT, Price.Currency);
        return totalWithVATInOriginalCurrency.ConvertTo(targetCurrency);
    }

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
