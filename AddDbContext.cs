using Microsoft.EntityFrameworkCore;
using ProduitsAPI;

public class AppDbContext : DbContext
{
    public DbSet<Produit> Produits { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
