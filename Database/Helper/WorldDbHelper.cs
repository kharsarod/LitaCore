using Database.Context;
using Database.Item;
using Database.MapEntity;
using Database.MonsterData;
using Database.Player;
using Database.Server;
using Database.ShopEntity;
using Database.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helper
{
    public static class WorldDbHelper
    {
        public static async Task<List<Item.Item>> LoadAllItemsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Items.Include(i => i.Translations)
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Item.Item>();
            }
        }

        public static async Task<List<Map>> LoadAllMapsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Maps.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Map>();
            }
        }

        public static async Task<BCard> LoadBCardByItemId(int itemId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.FirstOrDefaultAsync(b => b.ItemId == itemId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<List<BCard>> LoadBCardsByItemId(int itemId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.Where(b => b.ItemId == itemId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<BCard>();
            }
        }

        public static async Task<Map> LoadByMapId(int mapId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Maps.FirstOrDefaultAsync(m => m.Id == mapId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task UpdateMapAsync(Map map)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Maps.Update(map);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertPortalAsync(Portal portal)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Portals.AddAsync(portal);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<Portal>> LoadAllPortalsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Portals.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Portal>();
            }
        }

        public static async Task InsertPortalsAsync(List<Portal> portals)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Portals.AddRangeAsync(portals);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task LoadPortalByMapId(int mapId)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Portals.FirstOrDefaultAsync(p => p.FromMapId == mapId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdatePortalAsync(Portal portal)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Portals.Update(portal);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeletePortalAsync(Portal portal)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Portals.Remove(portal);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertMonsterAsync(Monster monster)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Monsters.AddAsync(monster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertMonstersAsync(List<Monster> monsters)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Monsters.AddRangeAsync(monsters);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<Monster>> LoadAllMonstersAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Monsters.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Monster>();
            }
        }

        public static async Task<short> GetAllMonsterVNums()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Monsters.MaxAsync(m => m.VNum);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException)
                {
                    return 0;
                }
                Log.Error(e.ToString());
                return 0;
            }
        }

        public static async Task<int> GetAllMonsterIds()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Monsters.MaxAsync(m => m.MonsterId);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException)
                {
                    return 0;
                }
                Log.Error(e.Message);
                return 0;
            }
        }

        public static async Task UpdateMonsterAsync(Monster monster)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Monsters.Update(monster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteMonsterAsync(Monster monster)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Monsters.Remove(monster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task LoadMonsterById(int id)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Monsters.FirstOrDefaultAsync(m => m.MonsterId == id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task LoadMonsterByVNum(short vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Monsters.FirstOrDefaultAsync(m => m.VNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcAsync(Npc npc)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Npcs.AddAsync(npc);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcsAsync(List<Npc> npcs)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Npcs.AddRangeAsync(npcs);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<Npc>> LoadAllNpcsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Npcs.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Npc>();
            }
        }

        public static async Task UpdateNpcAsync(Npc npc)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Npcs.Update(npc);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteNpcAsync(Npc npc)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Npcs.Remove(npc);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<Npc> LoadNpcById(int id)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Npcs.FirstOrDefaultAsync(n => n.NpcId == id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<Npc> LoadNpcByMapId(int mapId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Npcs.FirstOrDefaultAsync(n => n.MapId == mapId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task LoadNpcByVNum(short vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Npcs.FirstOrDefaultAsync(n => n.VNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<Shop>> LoadAllShopsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Shops.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Shop>();
            }
        }

        public static async Task InsertShopAsync(Shop shop)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Shops.AddAsync(shop);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertShopsAsync(List<Shop> shops)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Shops.AddRangeAsync(shops);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateShopAsync(Shop shop)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Shops.Update(shop);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteShopAsync(Shop shop)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Shops.Remove(shop);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task LoadShopById(int id)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Shops.FirstOrDefaultAsync(s => s.ShopId == id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<Shop> LoadShopByNpcId(int npcId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Shops.FirstOrDefaultAsync(s => s.NpcId == npcId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<List<ShopTranslations>> LoadAllShopTranslationsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.ShopTranslations.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ShopTranslations>();
            }
        }

        public static async Task<List<ShopItem>> LoadAllShopItemsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.ShopItems.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ShopItem>();
            }
        }

        public static async Task InsertShopItemAsync(ShopItem shopItem)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.ShopItems.AddAsync(shopItem);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertShopItemsAsync(List<ShopItem> shopItems)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.ShopItems.AddRangeAsync(shopItems);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateShopItemAsync(ShopItem shopItem)
        {
            try
            {
                using var context = new WorldDbContext();
                context.ShopItems.Update(shopItem);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteShopItemAsync(ShopItem shopItem)
        {
            try
            {
                using var context = new WorldDbContext();
                context.ShopItems.Remove(shopItem);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<ShopItem>> LoadShopItemsByShopId(int shopId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.ShopItems.Where(si => si.ShopId == shopId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ShopItem>();
            }
        }

        public static async Task<List<NpcMonster>> LoadAllNpcMonstersAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.NpcMonsters.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<NpcMonster>();
            }
        }

        public static async Task<NpcMonster> LoadNpcMonsterByVNumAsync(int vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.NpcMonsters.FirstOrDefaultAsync(nm => nm.VNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertNpcMonsterAsync(NpcMonster npcMonster)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.NpcMonsters.AddAsync(npcMonster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateNpcMonsterAsync(NpcMonster npcMonster)
        {
            try
            {
                using var context = new WorldDbContext();
                context.NpcMonsters.Update(npcMonster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcMonstersAsync(List<NpcMonster> npcMonsters)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.NpcMonsters.AddRangeAsync(npcMonsters);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteNpcMonsterByVNum(int vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                var monster = await context.NpcMonsters.FirstOrDefaultAsync(m => m.VNum == vNum);
                context.NpcMonsters.Remove(monster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcMonsterSkillAsync(NpcMonsterSkill npcMonsterSkill)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.NpcMonsterSkills.AddAsync(npcMonsterSkill);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcMonsterCardsAsync(List<BCard> npcMonsterCards)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.BCards.AddRangeAsync(npcMonsterCards);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcMonsterDropsAsync(List<DropData> npcMonsterDrops)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Drops.AddRangeAsync(npcMonsterDrops);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertNpcMonsterSkillsAsync(List<NpcMonsterSkill> npcMonsterSkills)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.NpcMonsterSkills.AddRangeAsync(npcMonsterSkills);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteNpcMonsterSkillAsync(NpcMonsterSkill npcMonsterSkill)
        {
            try
            {
                using var context = new WorldDbContext();
                context.NpcMonsterSkills.Remove(npcMonsterSkill);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertDropAsync(DropData drop)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Drops.AddAsync(drop);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<DropData> LoadDropByMonsterVNumAsync(int vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Drops.FirstOrDefaultAsync(d => d.VNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<DropData> LoadDropByMonsterAndItemVNumAsync(int vNum, int itemVNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Drops.FirstOrDefaultAsync(d => d.VNum == vNum && d.MonsterVNum == itemVNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertDropsAsync(List<DropData> drops)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Drops.AddRangeAsync(drops);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<DropData>> LoadAllDropsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Drops.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task DeleteDropAsync(DropData drop)
        {
            try
            {
                using var context = new WorldDbContext();
                context.Drops.Remove(drop);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<IEnumerable<ComboData>> LoadComboByVNumHitAndEffect(short vNum, short hit, short effect)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Combos.Where(c => c.SkillVNum == vNum && c.Hit == hit && c.Effect == effect).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertCombosAsync(List<ComboData> combos)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Combos.AddRangeAsync(combos);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertSkillsAsync(List<SkillData> skills)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Skills.AddRangeAsync(skills);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertSkillsBcards(List<BCard> bcards)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.BCards.AddRangeAsync(bcards);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<SkillData>> LoadAllSkillsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Skills.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<SkillData> LoadSkillByVNumAsync(short vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Skills.FirstOrDefaultAsync(s => s.SkillVNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<BCard> LoadBCardBySkillVNumAsync(short vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.FirstOrDefaultAsync(s => s.SkillVNum == vNum);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }



        public static async Task<List<BCard>> LoadSkillBCardsBySkillVNumAsync(short vNum)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.Where(s => s.SkillVNum == vNum).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<Map> InsertMapAsync(Map map)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Maps.AddAsync(map);
                await context.SaveChangesAsync();
                return map;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<WorldLog> InsertWorldLogAsync(WorldLog worldLog)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.WorldLogs.AddAsync(worldLog);
                await context.SaveChangesAsync();
                return worldLog;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
        public static async Task<WorldLog> LoadWorldBySource(string source)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.WorldLogs.FirstOrDefaultAsync(wl => wl.Source == source);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<List<WorldLog>> LoadWorldLogsBySourceAsync(string source)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.WorldLogs.Where(wl => wl.Source == source).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<WorldLog>();
            }
        }

        public static async Task RemoveWorldLogBySourceAndType(string source, string type)
        {
            try
            {
                using var context = new WorldDbContext();
                var worldLog = await context.WorldLogs.FirstOrDefaultAsync(wl => wl.Source == source && wl.Type == type);
                if (worldLog != null)
                {
                    context.WorldLogs.Remove(worldLog);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task RemoveWorldLogBySource(string source)
        {
            try
            {
                using var context = new WorldDbContext();
                var worldLog = await context.WorldLogs.FirstOrDefaultAsync(wl => wl.Source == source);
                if (worldLog != null)
                {
                    context.WorldLogs.Remove(worldLog);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertBuffAsync(BuffData buff)
        {
            try
            {
                using var context = new WorldDbContext();
                await context.Buffs.AddAsync(buff);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<BuffData>> LoadAllBuffsAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Buffs.Include(x => x.Translations).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<BuffData>();
            }
        }

        public static async Task<BuffData> LoadBuffByIdAsync(short bId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.Buffs.Include(x => x.Translations).FirstOrDefaultAsync(b => b.BuffId == bId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
        public static async Task InsertBuffsAsync(List<BuffData> buffs)
        {
            // Insertar una lista de BuffData en la base de datos, pero sin duplicados y si ya existe, actualizarlo.
            try
            {
                using var context = new WorldDbContext();
                foreach (var buff in buffs)
                {
                    var existingBuff = await context.Buffs.FirstOrDefaultAsync(b => b.BuffId == buff.BuffId);
                    if (existingBuff != null)
                    {
                        // Actualizar el buff existente
                        context.Entry(existingBuff).CurrentValues.SetValues(buff);
                    }
                    else
                    {
                        // Insertar un nuevo buff
                        await context.Buffs.AddAsync(buff);
                    }
                }
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.InnerException?.Message ?? string.Empty);
            }
        }
        public static async Task DeleteBuffByBuffId(short buffId)
        {
            try
            {
                using var context = new WorldDbContext();
                var buff = await context.Buffs.FirstOrDefaultAsync(b => b.BuffId == buffId);
                if (buff != null)
                {
                    context.Buffs.Remove(buff);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertBuffBCardsAsync(List<BCard> bcards)
        {
            // Insertar una lista de BCard en la base de datos, pero sin duplicados y si ya existe, actualizarlo.
            try
            {
                using var context = new WorldDbContext();
                foreach (var bcard in bcards)
                {
                    var existingBCard = await context.BCards.FirstOrDefaultAsync(b => b.BCardId == bcard.BCardId);
                    if (existingBCard != null)
                    {
                        // Actualizar la BCard existente
                        context.Entry(existingBCard).CurrentValues.SetValues(bcard);
                    }
                    else
                    {
                        // Insertar una nueva BCard
                        await context.BCards.AddAsync(bcard);
                    }
                }
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<BCard>> LoadAllBcardsWithBuffIdAsync()
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.Where(b => b.BuffId.HasValue).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<BCard>();
            }
        }

        public static async Task<List<BCard>> LoadBuffBCardsByBuffIdAsync(short buffId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.Where(b => b.BuffId == buffId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<BCard>();
            }
        }

        public static async Task<BCard> LoadBuffBCardById(short buffId)
        {
            try
            {
                using var context = new WorldDbContext();
                return await context.BCards.FirstOrDefaultAsync(b => b.BuffId == buffId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}