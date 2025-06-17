using Database.Context;
using Database.Player;
using Database.Social;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helper
{
    public static class CharacterDbHelper
    {
        public static async Task<List<CharacterItem>> LoadInventoryAsync(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterItems.Where(ci => ci.CharacterId == characterId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<CharacterItem>();
            }
        }

        public static async Task InsertItemAsync(CharacterItem item)
        {
            try
            {
                using var context = new CharacterDbContext();
                await context.CharacterItems.AddAsync(item);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertAsync(Character character)
        {
            try
            {
                using var context = new CharacterDbContext();
                await context.Characters.AddAsync(character);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateItemsAsync(List<CharacterItem> items)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.CharacterItems.UpdateRange(items);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateItemAsync(CharacterItem item)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.CharacterItems.Update(item);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateAsync(CharacterItem item)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.CharacterItems.Update(item);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateAsync(Character character)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<Character> LoadByNameAsync(string name)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Characters.FirstOrDefaultAsync(c => c.Name == name);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task RemoveAsync<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                using var context = new CharacterDbContext();
                if (entity is null) return;
                context.Set<TEntity>().Remove(entity);
                await context.SaveChangesAsync();
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
                using var context = new CharacterDbContext();
                return await context.Characters.Where(c => c.AccountId == accountId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Character>();
            }
        }

        public static async Task<List<Character>> LoadCharactersBySlotAndAccountId(byte slot, int accountId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Characters.Where(c => c.Slot == slot && c.AccountId == accountId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Character>();
            }
        }

        public static async Task<List<Character>> LoadCharactersByServerIdAndAccountId(int serverId, int accountId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Characters.Where(c => c.ServerId == serverId && c.AccountId == accountId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Character>();
            }
        }

        public static async Task<CharacterItem> LoadItemById(int id, int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterItems.FirstOrDefaultAsync(ci => ci.CharacterId == characterId && ci.ItemId == id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<Character> LoadCharacterById(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Characters.FirstOrDefaultAsync(c => c.Id == characterId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<CharacterItem> LoadItemBySlot(int slot, int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterItems.FirstOrDefaultAsync(ci => ci.CharacterId == characterId && ci.Slot == slot);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<CharacterItem> LoadItemById2(int characterId, int id)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterItems.FirstOrDefaultAsync(ci => ci.CharacterId == characterId && ci.Id == id);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<List<CharacterItem>> LoadCharacterItemsByCharacterId(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterItems.Where(ci => ci.CharacterId == characterId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<CharacterItem>();
            }
        }

        public static async Task<Character> LoadCharacterBySlotAndAccountId(byte slot, int accountId, short serverId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Characters.FirstOrDefaultAsync(c => c.Slot == slot && c.AccountId == accountId && c.ServerId == serverId);
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
                using var context = new CharacterDbContext();
                context.Characters.Remove(character);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<Friend> LoadFriendByName(string name)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Friends.FirstOrDefaultAsync(f => f.FriendName == name);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<Friend> LoadFriendById(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Friends.FirstOrDefaultAsync(f => f.CharacterId == characterId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task<List<Friend>> LoadAllFriendsAsync()
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Friends.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<Friend>();
            }
        }

        public static async Task<Friend> VerifyIfFriendAlreadyAdded(Friend friend)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.Friends.FirstOrDefaultAsync(f => f.CharacterId == friend.CharacterId && f.FriendName == friend.FriendName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertFriendAsync(Friend friend)
        {
            try
            {
                using var context = new CharacterDbContext();
                await context.Friends.AddAsync(friend);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task RemoveFriendAsync(Friend friend)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.Friends.Remove(friend);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateFriendAsync(Friend friend)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.Friends.Update(friend);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<ActionBar>> LoadAllActionBarAsync()
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.ActionBars.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ActionBar>();
            }
        }

        public static async Task<List<ActionBar>> LoadAllActionBarAsyncByPlayerId(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.ActionBars.Where(ab => ab.CharacterId == characterId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ActionBar>();
            }
        }

        public static async Task<ActionBar> LoadActionBarByCharacterId(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.ActionBars.FirstOrDefaultAsync(ab => ab.CharacterId == characterId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertActionBarAsync(ActionBar actionBar)
        {
            try
            {
                using var context = new CharacterDbContext();
                if (await context.ActionBars.ContainsAsync(actionBar))
                {
                    return;
                }
                await context.ActionBars.AddAsync(actionBar);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateActionBarAsync(ActionBar actionBar)
        {
            try
            {
                using var context = new CharacterDbContext();

                context.ActionBars.Update(actionBar);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task InsertActionBarListAsync(List<ActionBar> actionBars)
        {
            try
            {
                using var context = new CharacterDbContext();

                var nuevos = actionBars.Where(ab => ab.Id == 0).ToList();
                if (!nuevos.Any())
                    return;

                await context.ActionBars.AddRangeAsync(nuevos);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.InnerException?.Message ?? e.Message);
            }
        }


        public static async Task UpdateActionBarListAsync(List<ActionBar> actionBars, int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();

                // 1. Cargar todas las ActionBars existentes del personaje
                var existentes = await context.ActionBars
                    .Where(x => x.CharacterId == characterId)
                    .ToListAsync();

                // 2. Eliminar todas las ActionBars existentes
                context.ActionBars.RemoveRange(existentes);

                // 4. Agregar las nuevas ActionBars
                context.ActionBars.AddRange(actionBars);

                // 5. Guardar cambios
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al actualizar la lista de ActionBars");
            }
        }

        public static async Task UpdateActionBarsListAsync(List<ActionBar> actionBars)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.ActionBars.UpdateRange(actionBars);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task RemoveActionBarListAsync(List<ActionBar> actionBars)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.ActionBars.RemoveRange(actionBars);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task DeleteActionBarAsync(ActionBar actionBar)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.ActionBars.Remove(actionBar);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<CharacterSkill>> LoadCharacterSkillsByCharacterIdAsync(int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterSkills.Where(cs => cs.CharacterId == characterId).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<CharacterSkill>();
            }
        }

        public static async Task<CharacterSkill> LoadCharacterSkillByCastId(int castId, int characterId)
        {
            try
            {
                using var context = new CharacterDbContext();
                return await context.CharacterSkills.FirstOrDefaultAsync(cs => cs.CastId == castId && cs.CharacterId == characterId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task InsertCharacterSkillAsync(CharacterSkill characterSkill)
        {
            try
            {
                using var context = new CharacterDbContext();
                await context.CharacterSkills.AddAsync(characterSkill);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateCharacterSkillsAsync(List<CharacterSkill> skillsToUpdate)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.CharacterSkills.UpdateRange(skillsToUpdate);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        
        public static async Task RemoveCharacterSkillsAsync(List<CharacterSkill> skillsToRemove)
        {
            try
            {
                using var context = new CharacterDbContext();
                context.CharacterSkills.RemoveRange(skillsToRemove);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<CharacterSkill>> InsertCharacterSkills(List<CharacterSkill> characterSkills)
        {
            try
            {
                using var context = new CharacterDbContext();
                await context.CharacterSkills.AddRangeAsync(characterSkills);
                await context.SaveChangesAsync();
                return characterSkills;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<CharacterSkill>();
            }
        }
    }
}