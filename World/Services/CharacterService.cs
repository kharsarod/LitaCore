using Database.Data.Repositories;
using Database.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Services
{
    public class CharacterService
    {
        private readonly CharacterRepository _repository;
        public CharacterService()
        {
            var dbContext = new AppDbContext();
            _repository = new CharacterRepository(dbContext);
        }

        public async Task Update(Character character) => await _repository.Update(character);
    }
}
