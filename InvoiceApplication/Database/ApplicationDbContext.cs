using InvoiceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApplication.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<InvoiceItem<Currency>> InvoiceItems { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Client> Clients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация на таблицата за InvoiceItem
            modelBuilder.Entity<InvoiceItem<Currency>>(entity =>
            {
                entity.HasKey(i => i.Name); // Примерен ключ
                entity.Property(i => i.Quantity).IsRequired();
                entity.Property(i => i.VATPercentage).HasDefaultValue(20);
                entity.Property(i => i.DiscountQuantityThreshold).HasDefaultValue(0);
                entity.Property(i => i.DiscountPercentage).HasDefaultValue(0);

                // Конфигуриране на Price като owned type
                entity.OwnsOne(i => i.Price, price =>
                {
                    price.Property(p => p.Amount).IsRequired();
                    price.HasOne(p => p.Currency)
                         .WithMany()
                         .HasForeignKey("CurrencyCode")
                         .IsRequired();
                });
            });

            // Конфигурация на таблицата за Currency
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Code); // Примерен ключ
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.ExchangeRateToBase).IsRequired();
            });

            // Конфигурация на таблицата за Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id); // Primary key
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.Email).IsRequired(false);
                entity.Property(c => c.Phone).IsRequired(false);
                entity.Property(c => c.Address).IsRequired(false);
            });

            modelBuilder.Entity<Client>().HasData(
                new Client("Test Testov", "test@abv.bg", "08932323", "Gladston 31, Stara Zagora") { Id = Guid.NewGuid().ToString() },
                new Client("Ivan Ivanov", "ivan@abv.bg", "08942433", "Ivan Asen II, Stara Zagora") { Id = Guid.NewGuid().ToString() }
            );

            modelBuilder.Entity<Currency>().HasData(
                new Currency("US Dollar", "USD", 1.0m),
                new Currency("Euro", "EUR", 0.85m),
                new Currency("Bulgarian Lev", "BGN", 1.95m)
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Конфигуриране на SQLite база данни
                optionsBuilder.UseSqlite("Data Source=InvoiceApp.db");
            }
        }

        public List<InvoiceItem<Currency>> GetAllInvoiceItems()
        {
            return InvoiceItems.Include(i => i.Price.Currency).ToList();
        }

    }
}


