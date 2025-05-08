using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InvoiceApplication.Database;
using InvoiceApplication.Models;
using InvoiceApplication.ViewModel;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ApplicationDbContext _dbContext;
    private List<Currency> _currencies;
    private MultiCurrencyInvoice _multiCurrencyInvoice = new MultiCurrencyInvoice();
    private MainWindowViewModel _viewModel;


    public MainWindow(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(dbContext);
        DataContext = _viewModel;
        _dbContext = dbContext;
    }

    private void AddInvoiceItem_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AddInvoiceItem();
    }

    private void DeleteInvoiceItem_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        var dataContext = button.DataContext;
        var item = ((dynamic)dataContext).Item as InvoiceItem<Currency>; // Extract the original item

        if (item != null)
        {
            _viewModel.DeleteInvoiceItem(item);
        }
    }

    private void OpenProductManagementWindow_Click(object sender, RoutedEventArgs e)
    {
        var productManagementWindow = new ProductManagementWindow(_dbContext)
        {
            OnProductAdded = () =>
            {
                // Обновяване на продуктите в MainWindowViewModel
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.LoadInvoiceItems();
                }
            }
        };
        productManagementWindow.ShowDialog();
    }



}