using System.Configuration;
using System.Data;
using System.Windows;

using InvoiceApplication.Models;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
}

public static class CurrencyInitializer
{
    public static List<Currency> GetDefaultCurrencies()
    {
        return new List<Currency>
        {
            new Currency("US Dollar", "USD", 1.0m),
            new Currency("Euro", "EUR", 0.85m),
            new Currency("Bulgarian Lev", "BGN", 1.95m)
        };
    }
}