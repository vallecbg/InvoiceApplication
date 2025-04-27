using System.Collections.Generic;
using System.Linq;

namespace InvoiceApplication.Models;

/// <summary>
/// Represents an invoice containing multiple items.
/// </summary>
public class Invoice<TCurrency> where TCurrency : Currency
{
    public List<InvoiceItem<TCurrency>> Items { get; }

    public Invoice()
    {
        Items = new List<InvoiceItem<TCurrency>>();
    }

    public void AddItem(InvoiceItem<TCurrency> item)
    {
        Items.Add(item);
    }

    /// <summary>
    /// Removes an item from the invoice.
    /// </summary>
    public void RemoveItem(InvoiceItem<TCurrency> item)
    {
        Items.Remove(item);
    }

    public SingleCurrencyAmount<TCurrency> GetTotal(TCurrency targetCurrency)
    {
        var totalAmount = Items
            .Select(item => item.GetTotalInSelectedCurrency(targetCurrency))
            .Sum(amount => amount.Amount);

        return new SingleCurrencyAmount<TCurrency>(totalAmount, targetCurrency);
    }

    public SingleCurrencyAmount<TTargetCurrency> GetTotalWithVATInSelectedCurrency<TTargetCurrency>(TTargetCurrency targetCurrency)
    where TTargetCurrency : Currency
    {
        var totalWithVAT = Items
            .Select(item => item.GetTotalWithVATInSelectedCurrency(targetCurrency))
            .Sum(amount => amount.Amount);

        return new SingleCurrencyAmount<TTargetCurrency>(totalWithVAT, targetCurrency);
    }
}
