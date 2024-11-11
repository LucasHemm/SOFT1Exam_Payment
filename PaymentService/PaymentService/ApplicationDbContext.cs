using Microsoft.EntityFrameworkCore;
using PaymentService.Models;


namespace PaymentService;

public class ApplicationDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentProcessInfo> PaymentProcessInfos { get; set; }

    // Constructor that accepts DbContextOptions
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure SQL Server if no options are provided (to avoid overriding options in tests)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=PaymentServiceDB;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;");
        }
    }
}