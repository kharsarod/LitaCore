using Database.Item;
using Database.MapEntity;
using Database.MonsterData;
using Database.Server;
using Database.ShopEntity;
using Database.World;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Context
{
    public class WorldDbContext : DbContext
    {
        public DbSet<Map> Maps { get; set; }
        public DbSet<Item.Item> Items { get; set; }
        public DbSet<ItemTranslation> ItemTranslations { get; set; }
        public DbSet<BCard> BCards { get; set; }
        public DbSet<Portal> Portals { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<Npc> Npcs { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<ShopTranslations> ShopTranslations { get; set; }
        public DbSet<NpcMonster> NpcMonsters { get; set; }
        public DbSet<NpcMonsterSkill> NpcMonsterSkills { get; set; }
        public DbSet<DropData> Drops { get; set; }
        public DbSet<SkillData> Skills { get; set; }
        public DbSet<ComboData> Combos { get; set; }
        public DbSet<WorldLog> WorldLogs { get; set; }
        public DbSet<BuffData> Buffs { get; set; }
        public DbSet<BuffTranslation> BuffTranslations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=litacore_world;user=litacore;password=litacore;";
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
            modelBuilder.Entity<Map>().ToTable("maps");
            modelBuilder.Entity<Item.Item>().ToTable("items");
            modelBuilder.Entity<ItemTranslation>().ToTable("items_translations");
            modelBuilder.Entity<BCard>().ToTable("bcards");
            modelBuilder.Entity<Portal>().ToTable("portals");
            modelBuilder.Entity<Monster>().ToTable("monsters");
            modelBuilder.Entity<Npc>().ToTable("npcs");
            modelBuilder.Entity<Shop>().ToTable("shops");
            modelBuilder.Entity<ShopItem>().ToTable("shop_items");
            modelBuilder.Entity<ShopTranslations>().ToTable("shop_translations");
            modelBuilder.Entity<NpcMonster>().ToTable("npc_monsters");
            modelBuilder.Entity<NpcMonsterSkill>().ToTable("npc_monster_skills");
            modelBuilder.Entity<DropData>().ToTable("drops");
            modelBuilder.Entity<SkillData>().ToTable("skills");
            modelBuilder.Entity<ComboData>().ToTable("combos");
            modelBuilder.Entity<WorldLog>().ToTable("world_logs");
            modelBuilder.Entity<BuffData>().ToTable("buffs");
            modelBuilder.Entity<BuffTranslation>().ToTable("buffs_translations");
        }

        public static async Task MapSaveChangesAsync(Map map)
        {
            try
            {
                using (var context = new WorldDbContext())
                {
                    context.Maps.Update(map);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}