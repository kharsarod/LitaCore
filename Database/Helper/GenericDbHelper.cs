using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helper
{
    public static class GenericDbHelper
    {
        public static async Task InsertAsync<TEntity, TContext>(TEntity entity)
            where TEntity : class
            where TContext : DbContext, new()
        {
            try
            {
                using var context = new TContext();
                await context.Set<TEntity>().AddAsync(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.InnerException?.Message ?? "No inner exception");
            }
        }

        public static async Task InsertListAsync<TEntity, TContext>(List<TEntity> entityList)
            where TEntity : class
            where TContext : DbContext, new()
        {
            try
            {
                using var context = new TContext();
                await context.Set<TEntity>().AddRangeAsync(entityList);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task RemoveAsync<TEntity, TContext>(TEntity entity)
            where TEntity : class
            where TContext : DbContext, new()
        {
            try
            {
                using var context = new TContext();
                context.Set<TEntity>().Remove(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateAsync<TEntity, TContext>(TEntity entity)
            where TEntity : class
            where TContext : DbContext, new()
        {
            try
            {
                using var context = new TContext();
                context.Set<TEntity>().Update(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<TEntity> LoadByIdAsync<TEntity, TContext>(int id)
            where TEntity : class
            where TContext : DbContext, new()
        {
            try
            {
                using var context = new TContext();
                return await context.Set<TEntity>().FindAsync(id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}