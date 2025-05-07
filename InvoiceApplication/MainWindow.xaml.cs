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

    
    private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        var dataContext = button.DataContext;
        var item = ((dynamic)dataContext).Item as InvoiceItem<Currency>; // Extract the original item

        if (item != null)
        {
            _multiCurrencyInvoice.RemoveItem(item);
            //UpdateAmountsListView();
        }
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {

        if (InvoiceItemComboBox.SelectedItem is InvoiceItem<Currency> selectedItem &&
        int.TryParse(ItemQuantityTextBox.Text, out var quantity) &&
        quantity > 0)
        {
            // Актуализиране на количеството на избрания артикул
            var updatedItem = new InvoiceItem<Currency>(
                selectedItem.Name,
                selectedItem.Price, // Използваме оригиналната цена
                quantity,           // Новото количество
                selectedItem.VATPercentage,
                selectedItem.DiscountQuantityThreshold,
                selectedItem.DiscountPercentage
            );

            // Добавяне на артикула към фактурата
            _multiCurrencyInvoice.AddItem(updatedItem);

            // Обновяване на визуализацията
            //UpdateAmountsListView();


            ItemQuantityTextBox.Clear();
            InvoiceItemComboBox.SelectedIndex = -1; // Нулиране на избора
            InvoiceItemComboBox.Text = "Choose product..."; // Връщане на placeholder текста
        }
        else
        {
            MessageBox.Show("Please select a valid item and enter a valid quantity.");
        }
    }

    private void OpenProductManagementWindow_Click(object sender, RoutedEventArgs e)
    {
        var productManagementWindow = new ProductManagementWindow(_dbContext)
        {
            //OnProductAdded = LoadInvoiceItems // Callback за обновяване на продуктите
        };
        productManagementWindow.ShowDialog();
    }



}