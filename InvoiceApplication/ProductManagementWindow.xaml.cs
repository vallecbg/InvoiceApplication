using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InvoiceApplication.Database;
using InvoiceApplication.Models;

namespace InvoiceApplication
{
    /// <summary>
    /// Interaction logic for ProductManagementWindow.xaml
    /// </summary>
    public partial class ProductManagementWindow : Window
    {
        private readonly ApplicationDbContext _dbContext;

        // Callback за обновяване на продуктите в MainWindow
        public Action OnProductAdded { get; set; }

        public ProductManagementWindow(ApplicationDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadCurrencies();
            LoadProducts();
        }

        private void LoadCurrencies()
        {
            // Зареждане на валутите от базата данни
            var currencies = _dbContext.Currencies.ToList();
            ProductCurrencyComboBox.ItemsSource = currencies;
            ProductCurrencyComboBox.SelectedIndex = 0; // Избиране на първата валута по подразбиране
        }

        private void LoadProducts()
        {
            // Зареждане на съществуващите продукти от базата данни
            var products = _dbContext.InvoiceItems.ToList();
            ProductsDataGrid.ItemsSource = products;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text) ||
                !decimal.TryParse(ProductPriceTextBox.Text, out var price) ||
                ProductCurrencyComboBox.SelectedItem is not Currency selectedCurrency)
            {
                MessageBox.Show("Please enter valid product details.");
                return;
            }

            // Създаване на нов продукт
            var newProduct = new InvoiceItem<Currency>(
                ProductNameTextBox.Text,
                new SingleCurrencyAmount<Currency>(price, selectedCurrency),
                quantity: 0 // Количеството е 0, защото това е само дефиниция на продукт
            );

            // Добавяне в базата данни
            _dbContext.InvoiceItems.Add(newProduct);
            _dbContext.SaveChanges();

            // Обновяване на таблицата с продукти
            LoadProducts();

            // Извикване на callback за обновяване на продуктите в MainWindow
            OnProductAdded?.Invoke();

            // Изчистване на текстовите полета
            ProductNameTextBox.Clear();
            ProductPriceTextBox.Clear();
            ProductCurrencyComboBox.SelectedIndex = 0;
        }
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is InvoiceItem<Currency> product)
            {
                // Потвърждение за изтриване
                var result = MessageBox.Show($"Are you sure you want to delete the product '{product.Name}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Изтриване на продукта от базата данни
                    _dbContext.InvoiceItems.Remove(product);
                    _dbContext.SaveChanges();

                    // Обновяване на таблицата с продукти
                    LoadProducts();

                    // Уведомяване на главния прозорец за промяната
                    OnProductAdded?.Invoke();
                }
            }
        }



    }
}
