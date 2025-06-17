using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Player
{
    public class LoginSession
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string Username { get; set; }
        public DateTime Expiration { get; set; }
    }
}