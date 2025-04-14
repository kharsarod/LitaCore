using Database.Item;
using Database.Player;
using Database.TokenDb;
using Database.World;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<SessionTokens> Tokens { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemTranslation> ItemTranslations { get; set; }

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
        modelBuilder.Entity<Character>().ToTable("characters");
        modelBuilder.Entity<Map>().ToTable("maps");
        modelBuilder.Entity<SessionTokens>().ToTable("session_tokens");
        modelBuilder.Entity<Item>().ToTable("items");
        modelBuilder.Entity<ItemTranslation>().ToTable("items_translations");
    }

    public static async Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        try
        {
            using (var context = new AppDbContext())
            {
                await context.Set<TEntity>().AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task<List<Map>> LoadAllMapsAsync()
    {
        try
        {
            using (var context = new AppDbContext())
            {
                return await context.Maps.ToListAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return new List<Map>();
        }
    }

    public static async Task<List<SessionTokens>> LoadAllTokensAsync()
    {
        try
        {
            using (var context = new AppDbContext())
            {
                return await context.Tokens.ToListAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return new List<SessionTokens>();
        }
    }

    public static async Task DeleteTokenAsync(string username)
    {
        try
        {
            using (var context = new AppDbContext())
            {
                var token = await context.Tokens.FirstOrDefaultAsync(t => t.Username == username);
                context.Tokens.Remove(token);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task<SessionTokens> LoadTokenAsync(string username)
    {
        try
        {
            using (var context = new AppDbContext())
            {
                return await context.Tokens.FirstOrDefaultAsync(t => t.Username == username);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return null;
        }
    }

    public static async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        try
        {
            using (var context = new AppDbContext())
            {
                context.Set<TEntity>().Update(entity);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task LoadByIdAsync<TEntity>(int id) where TEntity : class
    {
        try
        {
            using (var context = new AppDbContext())
            {
                await context.Set<TEntity>().FindAsync(id);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task LoadByNameAsync<TEntity>(string name) where TEntity : class
    {
        try
        {
            using (var context = new AppDbContext())
            {
                await context.Set<TEntity>().FindAsync(name);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task LoadByAccountIdAsync<TEntity>(int accountId) where TEntity : class
    {
        try
        {
            using (var context = new AppDbContext())
            {
                await context.Set<TEntity>().FindAsync(accountId);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    public static async Task<List<Character>> LoadCharactersByAccountId(int accountId)
    {
        try
        {
            using (var context = new AppDbContext())
            {
                return await context.Characters.Where(c => c.AccountId == accountId).ToListAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return new List<Character>();
        }
    }

    public static async Task<Character> LoadCharacterBySlot(byte slot)
    {
        try
        {
            using (var context = new AppDbContext())
            {
                return await context.Characters.FirstOrDefaultAsync(c => c.Slot == slot);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return null;
        }
    }

    public static async Task DeleteCharacterAsync(Character character)
    {
        try
        {
            using (var context = new AppDbContext())
            {
                context.Characters.Remove(character);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}
