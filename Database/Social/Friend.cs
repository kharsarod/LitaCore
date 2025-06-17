using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Social
{
    public class Friend
    {
        public int Id { get; set; }
        public short CharacterId { get; set; }
        public short FriendCharacterId { get; set; }
        public bool Married { get; set; }
        public string FriendName { get; set; }
        public int AccountId { get; set; }
    }
}