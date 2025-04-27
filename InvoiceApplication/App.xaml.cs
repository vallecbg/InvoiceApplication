using System.Configuration;
using System.Data;
using System.Windows;
using InvoiceApplication.Database;
using InvoiceApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Конфигуриране на SQLite база данни
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=InvoiceApp.db"));

        // Регистриране на прозореца
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        // Стартиране на главния прозорец
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
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