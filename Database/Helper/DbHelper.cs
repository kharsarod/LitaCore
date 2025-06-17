using Database.Context;
using Database.Item;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helper
{
    public static class DbHelper
    {
        public static async Task InsertAsync<TContext, TEntity>(TEntity entity)
            where TContext : DbContext, new()
            where TEntity : class
        {
            try
            {
                using (var context = new TContext())
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

        public static async Task InsertListAsync<TContext, TEntity>(List<TEntity> entityList)
            where TContext : DbContext, new()
            where TEntity : class
        {
            try
            {
                using (var context = new TContext())
                {
                    await context.Set<TEntity>().AddRangeAsync(entityList);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<Item.Item>> LoadAllItemsAsync()
        {
            try
            {
                using (var context = new WorldDbContext())
                {
                    return await context.Items.Include(i => i.Translations).AsNoTracking().ToListAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Item.Item>();
            }
        }

        public static async Task UpdateAsync<TContext, TEntity>(TEntity entity)
            where TContext : DbContext, new()
            where TEntity : class
        {
            try
            {
                using (var context = new TContext())
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
    }
}