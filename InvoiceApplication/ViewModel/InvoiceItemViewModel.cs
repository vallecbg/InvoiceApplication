using InvoiceApplication.Models;

namespace InvoiceApplication.ViewModel;

public class InvoiceItemViewModel
{
    public InvoiceItem<Currency> Item { get; set; } // Оригиналният обект
    public string Name { get; set; }
    public string OriginalPrice { get; set; }
    public string PriceInSelectedCurrency { get; set; }
    public int Quantity { get; set; }
    public string Discount { get; set; }
    public string Total { get; set; }
    public string VATAmount { get; set; }
    public string TotalWithVAT { get; set; }
}
