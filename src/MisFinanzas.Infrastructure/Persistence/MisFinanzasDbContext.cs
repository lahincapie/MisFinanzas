using Microsoft.EntityFrameworkCore;
using MisFinanzas.Domain.Categories;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;
using MisFinanzas.Domain.PaymentMethods;

namespace MisFinanzas.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de Entity Framework Core: representa la base de datos en código.
    /// Cada DbSet declara una tabla. Por aquí pasan todas las consultas y guardados.
    /// </summary>
    public class MisFinanzasDbContext : DbContext
    {
        public MisFinanzasDbContext(DbContextOptions<MisFinanzasDbContext> options)
            : base(options)
        {
        }

        // Categorías
        public DbSet<Category> Categories => Set<Category>();

        // Gastos y su cadena mensual
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseMonthly> ExpenseMonthlies => Set<ExpenseMonthly>();
        public DbSet<ExpensePayment> ExpensePayments => Set<ExpensePayment>();

        // Ingresos y su cadena mensual
        public DbSet<Income> Incomes => Set<Income>();
        public DbSet<IncomeMonthly> IncomeMonthlies => Set<IncomeMonthly>();
        public DbSet<IncomeReceipt> IncomeReceipts => Set<IncomeReceipt>();

        // Catálogo de medios de pago
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1) RowVersion: sello de concurrencia en todas las entidades que lo tienen
            modelBuilder.Entity<Category>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<Expense>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<ExpenseMonthly>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<ExpensePayment>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<Income>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<IncomeMonthly>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<IncomeReceipt>().Property(e => e.RowVersion).IsRowVersion();
            modelBuilder.Entity<PaymentMethod>().Property(e => e.RowVersion).IsRowVersion();

            // 2) Dinero: precisión exacta (18 dígitos totales, 2 decimales)
            modelBuilder.Entity<Expense>().Property(e => e.ExpectedAmount).HasPrecision(18, 2);
            modelBuilder.Entity<ExpensePayment>().Property(e => e.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Income>().Property(e => e.ExpectedAmount).HasPrecision(18, 2);
            modelBuilder.Entity<IncomeReceipt>().Property(e => e.Amount).HasPrecision(18, 2);

            // 3) Relaciones: al borrar el "padre", no borrar en cascada al "hijo"
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpensePayment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }

}