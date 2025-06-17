using Database.Player;
using Database.Social;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Context
{
    public class CharacterDbContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterItem> CharacterItems { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<ActionBar> ActionBars { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=litacore_characters;user=litacore;password=litacore;";
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
            modelBuilder.Entity<CharacterItem>().ToTable("character_items");
            modelBuilder.Entity<Friend>().ToTable("friends");
            modelBuilder.Entity<ActionBar>().ToTable("actionbars");
            modelBuilder.Entity<CharacterSkill>().ToTable("character_skills");
        }
    }
}