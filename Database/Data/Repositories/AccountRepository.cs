using Database.Player;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Data.Repositories
{
    public class AccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> LoadByName(string name)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == name);
            await _context.Entry(account).ReloadAsync();
            return account;
        }

        public async Task Save(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Insert(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByName(string name)
        {
            var account = await LoadByName(name);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}
