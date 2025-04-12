using Database.Data.Repositories;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Database.Commands
{
    public class AccountCreate
    {
        public static async Task CreateAccount(string name, string password, byte rank)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password)) return;

            var acc = new AccountRepository(new AppDbContext());
            SHA512 sHA512 = SHA512.Create();
            await acc.Insert(new Player.Account() { Username = name, Password = Cryptography.ToSha512(password), Rank = rank });

            Log.Information($"Created account {name} with rank {rank}");
        }
    }
}
