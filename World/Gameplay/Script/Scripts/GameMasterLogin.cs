using Enum.Main.ChatEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.Script.Objects;
using World.Gameplay.Script.ScriptAttributes;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Gameplay.Script.Scripts
{
    [Script("GameMasterLogin")]
    public class GameMasterLogin : PlayerScript
    {
        private ClientSession Session => Player.Session;
        public GameMasterLogin(Player player) : base(player) { }

        public async override Task OnLogin()
        {
            if (Session.Account.Rank > 0)
            {
                await Session.Player.ChatSay("Usa $commands para ver la lista de comandos GM.", ChatColor.Yellow);
            }
        }
    }
}
