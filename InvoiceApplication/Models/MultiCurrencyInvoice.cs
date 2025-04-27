using System.Collections.Generic;
using System.Linq;

namespace InvoiceApplication.Models;

/// <summary>
/// Represents an invoice that supports multiple currencies.
/// </summary>
public class MultiCurrencyInvoice
{
    private readonly Dictionary<string, object> _invoices;

    public MultiCurrencyInvoice()
    {
        _invoices = new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds an item to the appropriate invoice based on its currency.
    /// </summary>
    public void AddItem<TCurrency>(InvoiceItem<TCurrency> item) where TCurrency : Currency
    {
        var currencyCode = item.Price.Currency.Code;

        if (!_invoices.ContainsKey(currencyCode))
        {
            _invoices[currencyCode] = new Invoice<TCurrency>();
        }

        var invoice = (Invoice<TCurrency>)_invoices[currencyCode];
        invoice.AddItem(item);
    }

    /// <summary>
    /// Gets the total amount for all invoices in the specified target currency.
    /// </summary>
    public SingleCurrencyAmount<Currency> GetTotal(Currency targetCurrency)
    {
        decimal totalAmount = 0;

        foreach (var invoice in _invoices.Values)
        {
            var invoiceType = invoice.GetType();
            var method = invoiceType.GetMethod("GetTotal");
            var result = method.Invoke(invoice, new object[] { targetCurrency });

            var amountProperty = result.GetType().GetProperty("Amount");
            totalAmount += (decimal)amountProperty.GetValue(result);
        }

        return new SingleCurrencyAmount<Currency>(totalAmount, targetCurrency);
    }

    /// <summary>
    /// Gets the total amount including VAT for all invoices in the specified target currency.
    /// </summary>
    public SingleCurrencyAmount<Currency> GetTotalWithVAT(Currency targetCurrency)
    {
        decimal totalWithVAT = 0;

        foreach (var invoice in _invoices.Values)
        {
            var invoiceType = invoice.GetType();
            var method = invoiceType.GetMethod("GetTotalWithVATInSelectedCurrency");
            var constructedMethod = method.MakeGenericMethod(targetCurrency.GetType());
            var result = constructedMethod.Invoke(invoice, new object[] { targetCurrency });

            var amountProperty = result.GetType().GetProperty("Amount");
            totalWithVAT += (decimal)amountProperty.GetValue(result);
        }

        return new SingleCurrencyAmount<Currency>(totalWithVAT, targetCurrency);
    }

    public IEnumerable<InvoiceItem<Currency>> GetAllItems()
    {
        return _invoices.Values
            .SelectMany(invoice =>
            {
                var invoiceType = invoice.GetType();
                var itemsProperty = invoiceType.GetProperty("Items");
                return (IEnumerable<InvoiceItem<Currency>>)itemsProperty.GetValue(invoice);
            });
    }
}