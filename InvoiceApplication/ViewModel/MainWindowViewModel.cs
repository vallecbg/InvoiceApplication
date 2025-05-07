using InvoiceApplication.Database;
using InvoiceApplication.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace InvoiceApplication.ViewModel
{
    /// <summary>
    /// ViewModel for the MainWindow.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly MultiCurrencyInvoice _multiCurrencyInvoice;
        public MainWindowViewModel()
        {
            Currencies = new ObservableCollection<Currency>();
            Clients = new ObservableCollection<Client>();
            InvoiceItems = new ObservableCollection<InvoiceItem<Currency>>();
        }
        public MainWindowViewModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            // Инициализация на MultiCurrencyInvoice
            _multiCurrencyInvoice = new MultiCurrencyInvoice();

            // Зареждане на данни
            LoadCurrencies();
            LoadClients();
            LoadInvoiceItems();

            
        }

        public ObservableCollection<Currency> Currencies { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<InvoiceItem<Currency>> InvoiceItems { get; set; }

        private Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged();
                UpdateCurrentInvoiceItems();
                UpdateAmountsListView();
            }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        private InvoiceItem<Currency> _selectedInvoiceItem;
        public InvoiceItem<Currency> SelectedInvoiceItem
        {
            get => _selectedInvoiceItem;
            set
            {
                _selectedInvoiceItem = value;
                OnPropertyChanged();
            }
        }

        private int _selectedQuantity;
        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                _selectedQuantity = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InvoiceItemViewModel> CurrentInvoiceItems { get; set; } = new ObservableCollection<InvoiceItemViewModel>();

        private void UpdateCurrentInvoiceItems()
        {
            CurrentInvoiceItems.Clear();
            foreach (var item in _multiCurrencyInvoice.GetAllItems())
            {
                var viewModel = new InvoiceItemViewModel
                {
                    Item = item,
                    Name = item.Name,
                    OriginalPrice = $"{item.Price.Amount:F2} {item.Price.Currency.Code}",
                    PriceInSelectedCurrency = SelectedCurrency != null
                        ? $"{item.GetPriceInSelectedCurrency(SelectedCurrency).Amount:F2} {SelectedCurrency.Code}"
                        : "N/A",
                    Quantity = item.Quantity,
                    Discount = SelectedCurrency != null
                        ? $"{item.GetDiscountAmountInSelectedCurrency(SelectedCurrency).Amount:F2} {SelectedCurrency.Code}"
                        : "N/A",
                    Total = SelectedCurrency != null
                        ? $"{item.GetTotalInSelectedCurrency(SelectedCurrency).Amount:F2} {SelectedCurrency.Code}"
                        : "N/A",
                    VATAmount = SelectedCurrency != null
                        ? $"{item.GetVATAmountInSelectedCurrency(SelectedCurrency).Amount:F2} {SelectedCurrency.Code}"
                        : "N/A",
                    TotalWithVAT = SelectedCurrency != null
                        ? $"{item.GetTotalWithVATInSelectedCurrency(SelectedCurrency).Amount:F2} {SelectedCurrency.Code}"
                        : "N/A"
                };

                CurrentInvoiceItems.Add(viewModel);
            }
        }



        public ICommand AddInvoiceItemCommand { get; }
        public void AddInvoiceItem()
        {
            Debug.WriteLine($"SelectedInvoiceItem: {SelectedInvoiceItem?.Name ?? "null"}");
            Debug.WriteLine($"SelectedQuantity: {SelectedQuantity}");

            if (SelectedInvoiceItem != null && SelectedQuantity > 0)
            {
                Debug.WriteLine("Entering the if block...");

                var updatedItem = new InvoiceItem<Currency>(
                    SelectedInvoiceItem.Name,
                    SelectedInvoiceItem.Price,
                    SelectedQuantity,
                    SelectedInvoiceItem.VATPercentage,
                    SelectedInvoiceItem.DiscountQuantityThreshold,
                    SelectedInvoiceItem.DiscountPercentage
                );

                _multiCurrencyInvoice.AddItem(updatedItem);
                UpdateCurrentInvoiceItems();
                UpdateAmountsListView();
            }
            else
            {
                Debug.WriteLine("Condition not met. Exiting the if block.");
            }
        }

        public ICommand DeleteInvoiceItemCommand { get; }
        public void DeleteInvoiceItem(InvoiceItem<Currency> item)
        {
            if (item != null)
            {
                _multiCurrencyInvoice.RemoveItem(item);
                UpdateCurrentInvoiceItems();
                UpdateAmountsListView();
            }
        }

        private void LoadCurrencies()
        {
            Currencies = new ObservableCollection<Currency>(_dbContext.Currencies.ToList());
            SelectedCurrency = Currencies.FirstOrDefault();
        }

        private void LoadClients()
        {
            Clients = new ObservableCollection<Client>(_dbContext.Clients.ToList());
            SelectedClient = Clients.FirstOrDefault();
        }

        private void LoadInvoiceItems()
        {
            InvoiceItems = new ObservableCollection<InvoiceItem<Currency>>(_dbContext.GetAllInvoiceItems());
        }
        private void UpdateAmountsListView()
        {
            if (SelectedCurrency == null) return;

            var targetCurrency = SelectedCurrency;

            TotalText = $"Total: {_multiCurrencyInvoice.GetTotal(targetCurrency).Amount:F2} {targetCurrency.Code}";
            TotalWithVATText = $"Total (With VAT): {_multiCurrencyInvoice.GetTotalWithVAT(targetCurrency).Amount:F2} {targetCurrency.Code}";
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(TotalWithVATText));
        }

        public string TotalText { get; set; }
        public string TotalWithVATText { get; set; }
    }
}

