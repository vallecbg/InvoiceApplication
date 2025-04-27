using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InvoiceApplication.Models;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Currency> _currencies;
    private MultiCurrencyInvoice _multiCurrencyInvoice = new MultiCurrencyInvoice();


    public MainWindow()
    {
        InitializeComponent();
        LoadCurrencies();
    }

    private void LoadCurrencies()
    {
        _currencies = CurrencyInitializer.GetDefaultCurrencies();
        BaseCurrencyComboBox.ItemsSource = _currencies;
        BaseCurrencyComboBox.SelectedIndex = 0; // Select the first currency by default

        // Set the same currencies for the item currency selection
        ItemCurrencyComboBox.ItemsSource = _currencies;
        ItemCurrencyComboBox.SelectedIndex = 0; // Select the first currency by default
    }

    private void UpdateAmountsListView()
    {
        var targetCurrency = (Currency)BaseCurrencyComboBox.SelectedItem;
        OriginalPriceColumn.Header = $"Price";
        ConvertedPriceColumn.Header = $"Price ({targetCurrency.Code})";
        TotalColumn.Header = $"Total";
        TotalConvertedColumn.Header = $"Total ({targetCurrency.Code})";

        // Обновяване на списъка с артикули
        InvoiceItemsListView.ItemsSource = null;

        var allItems = _multiCurrencyInvoice.GetAllItems()
            .Select(item => new
            {
                item.Name,
                OriginalPrice = $"{item.Price.Amount:F2} {item.Price.Currency.Code}", // Оригиналната цена с валута
                PriceInSelectedCurrency = $"{item.GetPriceInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}", // Конвертираната цена с валута
                item.Quantity,
                Total = $"{item.Total:F2} {item.Price.Currency.Code}", // Тотал с оригиналната валута
                TotalInSelectedCurrency = $"{item.GetTotalInSelectedCurrency(targetCurrency).Amount:F2} {targetCurrency.Code}" // Тотал в конвертираната валута
            })
            .ToList();

        InvoiceItemsListView.ItemsSource = allItems;

        // Изчисляване на общата сума
        var total = _multiCurrencyInvoice.GetTotal(targetCurrency);
        TotalTextBlock.Text = $"{total.Amount:F2} {targetCurrency.Code}";
    }





    private void BaseCurrencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateAmountsListView();
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ItemNameTextBox.Text) ||
            !decimal.TryParse(ItemPriceTextBox.Text, out var price) ||
            !int.TryParse(ItemQuantityTextBox.Text, out var quantity) ||
            quantity <= 0 ||
            ItemCurrencyComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please enter valid item details.");
            return;
        }

        var itemCurrency = (Currency)ItemCurrencyComboBox.SelectedItem;

        // Създаване на нов артикул с избраната валута
        var item = new InvoiceItem<Currency>(ItemNameTextBox.Text, new SingleCurrencyAmount<Currency>(price, itemCurrency), quantity);

        // Добавяне на артикула към MultiCurrencyInvoice
        _multiCurrencyInvoice.AddItem(item);

        // Обновяване на визуализацията
        UpdateAmountsListView();

        // Изчистване на текстовите полета
        ItemNameTextBox.Clear();
        ItemPriceTextBox.Clear();
        ItemQuantityTextBox.Clear();
        ItemCurrencyComboBox.SelectedIndex = 0;
    }



}