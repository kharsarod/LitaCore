using Database.Player;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Character> Characters { get; set; }

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
