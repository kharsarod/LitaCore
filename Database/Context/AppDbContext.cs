using Database.Item;
using Database.Player;
using Database.Server;
using Database.TokenDb;
using Database.World;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemTranslation> ItemTranslations { get; set; }
    public DbSet<BCard> BCards { get; set; }
    public DbSet<CharacterItem> CharacterItems { get; set; }
    public DbSet<LoginSession> LoginSessions { get; set; }
    public DbSet<ChannelInfo> Channels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "server=localhost;port=3306;database=litacore;user=litacore;password=litacore;";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()?.ToLower());
            }
        }

        modelBuilder.Entity<Character>().ToTable("characters");
        modelBuilder.Entity<Map>().ToTable("maps");
        modelBuilder.Entity<Item>().ToTable("items");
        modelBuilder.Entity<ItemTranslation>().ToTable("items_translations");
        modelBuilder.Entity<CharacterItem>().ToTable("character_items");
    }
}