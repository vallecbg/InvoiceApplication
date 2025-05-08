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
using InvoiceApplication.ViewModel;

namespace InvoiceApplication;

/// <summary>
/// Interaction logic for ProductManagementWindow.xaml
/// </summary>
public partial class ProductManagementWindow : Window
{
    private readonly ApplicationDbContext _dbContext;
    private MainWindowViewModel _viewModel;
    // Callback за обновяване на продуктите в MainWindow
    public Action OnProductAdded { get; set; }

    public ProductManagementWindow(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(dbContext);
        DataContext = _viewModel;
    }
    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text) ||
    !decimal.TryParse(ProductPriceTextBox.Text, out var price))
        {
            MessageBox.Show("Please enter valid product details.");
            return;
        }
        _viewModel.AddInvoiceProduct(price);

        OnProductAdded?.Invoke();

        ProductNameTextBox.Clear();
        ProductPriceTextBox.Clear();
     
    }
    private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is InvoiceItem<Currency> product)
        {
            // Потвърждение за изтриване
            var result = MessageBox.Show($"Are you sure you want to delete the product '{product.Name}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel.RemoveInvoiceProduct(product);
                OnProductAdded?.Invoke();
            }
        }
    }



}
