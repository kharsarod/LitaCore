using Database.Player;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "server=localhost;port=3306;database=litacore;user=litacore;password=litacore;";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach(var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLower());

            foreach(var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()?.ToLower());
            }
        }

        modelBuilder.Entity<Account>().ToTable("accounts");
    }
}
