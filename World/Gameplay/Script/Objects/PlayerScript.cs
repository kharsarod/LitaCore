using Database.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.Script.Interfaces;

namespace World.Gameplay.Script.Objects
{
    public abstract class PlayerScript : IScript
    {
        protected Player Player { get; private set; }

        protected PlayerScript(Player player)
        {
            Player = player;
        }

        public virtual Task Register() => Task.CompletedTask;
        public virtual Task OnLevelUp(byte level) => Task.CompletedTask;
        public virtual Task OnLogin() => Task.CompletedTask;
        public virtual Task OnJobLevelUp(byte level) => Task.CompletedTask;
        public virtual Task OnWalk() => Task.CompletedTask;
        public virtual Task OnUseSkill(SkillData skill) => Task.CompletedTask;
        public virtual Task OnReceiveBuff(BuffData buff) => Task.CompletedTask;
    }
}
