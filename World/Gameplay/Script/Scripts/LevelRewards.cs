using Database.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.Script.Objects;
using World.Gameplay.Script.ScriptAttributes;

namespace World.Gameplay.Script.Scripts
{
    [Script("LevelRewards")] // Esto identifica que es un script, si no lo tiene no se ejecutará.
    public class LevelRewards : PlayerScript // Se hereda siempre PlayerScript si se trata del jugador.
    {
        public LevelRewards(Player player) : base(player) { } // Se deja vacío xd

        public override async Task OnLevelUp(byte level) // Método que se ejecuta cuando el personaje sube de nivel.
        {
        }
    }
}
