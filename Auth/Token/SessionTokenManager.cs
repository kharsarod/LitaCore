using Database.TokenDb;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Token
{
    public static class SessionTokenManager
    {
        public static readonly Dictionary<string, SessionTokens> ActiveTokens = new();


        public static async void AddToken(SessionTokens token)
        {
            var _token = await AppDbContext.LoadTokenAsync(token.Username);
            if (_token is null)
            {
                await AppDbContext.InsertAsync(token);
                ActiveTokens[token.Username] = token;
            }
        }

        public static async Task<SessionTokens> ValidateToken(string username)
        {
            return ActiveTokens.ContainsKey(username) ? ActiveTokens[username] : await AppDbContext.LoadTokenAsync(username);
        }

        public static void CheckTokenExpiration()
        {
            Observable.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(async _ =>
                {
                    var expiredTokens = ActiveTokens.Where(x => x.Value.ExpiresAt < DateTime.Now).ToList();

                    foreach(var expiredToken in expiredTokens)
                    {
                        await RemoveToken(expiredToken.Key);
                    }
                });
        }

        public static async Task RemoveToken(string username)
        {
            await AppDbContext.DeleteTokenAsync(username);
        }
    }
}
