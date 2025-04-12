using Database.Player;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Data.Repositories
{
    public class CharacterRepository : RepositoryBase<Character>
    {
        private readonly AppDbContext _context;

        public CharacterRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Character?> LoadByName(string name)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(a => a.Name == name);
            await _context.Entry(character).ReloadAsync();
            return character;
        }

        public async Task Save(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByName(string name)
        {
            var character = await LoadByName(name);
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
        }

        public async Task LoadById(int id)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(a => a.Id == id);
            await _context.Entry(character).ReloadAsync();
        }

        public async Task Insert(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }
    }
}
