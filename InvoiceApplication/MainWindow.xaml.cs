using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InvoiceApplication.Database;
using InvoiceApplication.Models;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ApplicationDbContext _dbContext;
    private List<Currency> _currencies;
    private MultiCurrencyInvoice _multiCurrencyInvoice = new MultiCurrencyInvoice();


    public MainWindow(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        _dbContext = dbContext;
        LoadCurrencies();
        LoadInvoiceItems();
        LoadClients();
    }

    private void LoadInvoiceItems()
    {
        var items = _dbContext.GetAllInvoiceItems();
        InvoiceItemComboBox.ItemsSource = items;
    }


    private void LoadCurrencies()
    {
        // Зареждане на валутите от базата данни
        _currencies = _dbContext.Currencies.ToList();
        BaseCurrencyComboBox.ItemsSource = _currencies;
        BaseCurrencyComboBox.SelectedIndex = 0; // Select the first currency by default

        // Set the same currencies for the item currency selection
        ItemCurrencyComboBox.ItemsSource = _currencies;
        ItemCurrencyComboBox.SelectedIndex = 0; // Select the first currency by default
    }

    private void UpdateAmountsListView()
    {
        var targetCurrency = (Currency)BaseCurrencyComboBox.SelectedItem;

        // Задаване на заглавията на колоните
        OriginalPriceColumn.Header = "Price";
        ConvertedPriceColumn.Header = $"Price ({targetCurrency.Code})";
        TotalColumn.Header = $"Total ({targetCurrency.Code})";
        VATColumn.Header = $"VAT ({targetCurrency.Code})";
        TotalWithVATColumn.Header = $"Total (With VAT) ({targetCurrency.Code})";

        // Обновяване на списъка с артикули
        InvoiceItemsListView.ItemsSource = null;

        // Вземане само на избраните артикули
        var selectedItems = _multiCurrencyInvoice.GetAllItems()
            .Select(item => new
            {
                Item = item, // Оригиналният обект
                Name = item.Name,
                OriginalPrice = $"{item.Price.Amount:F2} {item.Price.Currency.Code}", // Оригиналната цена с валута
                PriceInSelectedCurrency = $"{item.GetPriceInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}", // Конвертираната цена с валута
                item.Quantity,
                Discount = $"{item.GetDiscountAmountInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}", // Отстъпка в конвертираната валута
                Total = $"{item.GetTotalInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}", // Тотал в конвертираната валута
                VATAmount = $"{item.GetVATAmountInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}", // ДДС в конвертираната валута
                TotalWithVAT = $"{item.GetTotalWithVATInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}" // Тотал с включен ДДС в конвертираната валута
            })
            .ToList();

        InvoiceItemsListView.ItemsSource = selectedItems;

        // Изчисляване на общата сума
        var total = _multiCurrencyInvoice.GetTotal(targetCurrency);
        var totalWithVAT = _multiCurrencyInvoice.GetTotalWithVAT(targetCurrency);
        TotalTextBlock.Text = $"Total: {total.Amount:F2} {targetCurrency.Code}";
        TotalWithVATTextBlock.Text = $"Total (With VAT): {totalWithVAT.Amount:F2} {targetCurrency.Code}";
    }


    private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        var dataContext = button.DataContext;
        var item = ((dynamic)dataContext).Item as InvoiceItem<Currency>; // Extract the original item

        if (item != null)
        {
            _multiCurrencyInvoice.RemoveItem(item);
            UpdateAmountsListView();
        }
    }

    private void BaseCurrencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateAmountsListView();
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
            UpdateAmountsListView();


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
            OnProductAdded = LoadInvoiceItems // Callback за обновяване на продуктите
        };
        productManagementWindow.ShowDialog();
    }
    private void LoadClients()
    {
        // Зареждане на клиентите от базата данни
        var clients = _dbContext.Clients.ToList();
        ClientComboBox.ItemsSource = clients;
        ClientComboBox.SelectedIndex = 0; // Избиране на първия клиент по подразбиране
    }

    private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ClientComboBox.SelectedItem is Client selectedClient)
        {
            MessageBox.Show($"Selected client: {selectedClient.Name}");
            // Тук можете да добавите логика за свързване на клиента с фактурата
        }
    }



}