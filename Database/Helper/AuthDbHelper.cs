using Database.Context;
using Database.Player;
using Database.Server;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helper
{
    public static class AuthDbHelper
    {
        public static async Task<List<ChannelInfo>> LoadAllChannelsAsync()
        {
            try
            {
                using var context = new AuthDbContext();
                return await context.Channels.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ChannelInfo>();
            }
        }

        public static async Task<List<Account>> LoadAllAccountsAsync()
        {
            try
            {
                using var context = new AuthDbContext();
                return await context.Accounts.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task UpdateChannelOnlinePlayers(int channelId, int onlinePlayers)
        {
            try
            {
                using var context = new AuthDbContext();
                var channel = await context.Channels.FirstOrDefaultAsync(c => c.ChannelId == channelId);
                if (channel != null)
                {
                    channel.OnlinePlayers = onlinePlayers;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<Account> LoadAccountByNameAsync(string name)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    return await context.Accounts.FirstOrDefaultAsync(a => a.Username == name);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public static async Task UpdateAsync(Account account)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    context.Accounts.Update(account);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UpdateChannelAsync(ChannelInfo channel)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    context.Channels.Update(channel);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task<List<BnInfo>> LoadAllScrollingMessagesAsync()
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    return await context.BnInfos.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<BnInfo>();
            }
        }

        public static async Task<List<ServerInfo>> LoadAllServersAsync()
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    return await context.Servers.ToListAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new List<ServerInfo>();
            }
        }

        public static async Task InsertAccountAsync(Account account)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    await context.Accounts.AddAsync(account);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static async Task UnbanAccountByNameAsync(string name)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    var account = await context.Accounts.FirstOrDefaultAsync(a => a.Username == name);
                    if (account != null)
                    {
                        account.IsBanned = false;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}