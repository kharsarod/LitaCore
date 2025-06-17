using Database.Player;
using Database.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Context
{
    public class AuthDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ChannelInfo> Channels { get; set; }
        public DbSet<BnInfo> BnInfos { get; set; }
        public DbSet<ServerInfo> Servers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=litacore_auth;user=litacore;password=litacore;";
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

            modelBuilder.Entity<Account>().ToTable("accounts");
            modelBuilder.Entity<ChannelInfo>().ToTable("channels");
            modelBuilder.Entity<BnInfo>().ToTable("scrolling_messages");
            modelBuilder.Entity<ServerInfo>().ToTable("servers");
        }
    }
}