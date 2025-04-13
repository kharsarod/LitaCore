using Enum.Main.OptionEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Network;

namespace World.Extensions
{
    public class PlayerPackets
    {
        private ClientSession _session;
        public PlayerPackets(ClientSession session)
        {
            _session = session;
        }

        public string GenerateLev()
        {
            var level = _session.Player.Level;
            var exp = level <= 99 ? _session.Player.Exp : _session.Player.Exp / 100;
            var jobLevel = _session.Player.JobLevel;
            var jobExp = _session.Player.JobExp;
            var heroLevel = _session.Player.HeroLevel;
            var heroExp = heroLevel <= 99 ? _session.Player.HeroExp : _session.Player.HeroExp / 100;

            return $"lev {level} {exp} {jobLevel} {jobExp} 1000 1500 10 15 {heroExp} {heroLevel} 15000 0";
        }

        public string GenerateStat()
        {
            int option = 0;

            var player = _session.Player;

            if (player.IsWhispBlocked) option += 1 << 3;
            if (player.IsBlockedHud) option += 1 << ((int)Option.LockHud - 1);
            if (player.IsBlockedHat) option += 1 << ((int)Option.DisableHat - 1);
            if (player.IsFamilyRequestBlocked) option += 1 << ((int)Option.FamilyRequestBlocked - 1);
            if (!player.CursorAimLock) option += 1 << ((int)Option.MouseAimLock - 1);
            if (player.IsMinilandInviteBlocked) option += 1 << ((int)Option.MinilandInviteBlocked - 1);
            if (player.IsExchangeBlocked) option += 1 << ((int)Option.ExchangeBlocked - 1);
            if (player.IsFriendRequestBlocked) option += 1 << ((int)Option.FriendRequestBlocked - 1);
            if (player.IsEmoticonBlocked) option += 1 << ((int)Option.EmoticonsBlocked - 1);
            if (player.IsHealthBlocked) option += 1 << ((int)Option.HpBlocked - 1);
            if (player.IsBlockedBuff) option += 1 << ((int)Option.BuffBlocked - 1);
            if (player.IsGroupRequestBlocked) option += 1 << ((int)Option.GroupRequestBlocked - 1);
            if (player.IsHeroChatBlocked) option += 1 << ((int)Option.HeroChatBlocked - 1);
            if (player.IsQuickGetUpBlocked) option += 1 << ((int)Option.QuickGetUp - 1);
            if (!player.IsPetAutoRelive) option += 1 << 6;
            if (!player.IsPartnerAutoRelive) option += 1 << 7;
            if (player.IsDisplayCdBlocked) option += 1 << ((int)Option.DisplayCD + 1);
            if (player.IsDisplayHealthBlocked) option += 1 << ((int)Option.DisplayHP + 1);

            return $"stat {player.Health} {player.MaxHealth} {player.Mana} {player.MaxMana} 0 {option}";
        }


        public string GeneratePlayerMove()
        {
            var speed = _session.Player.Speed;
            return $"cond 1 1 0 0 {speed}";
        }

        public string GenerateRage()
        {
            return $"rage 0 0";
        }

        public string GenerateScale()
        {
            var scale = _session.Player.Scale;
            return $"char_sc 1 1 {scale}";
        }

        public string GenerateFood()
        {
            return "food 0";
        }

        public string GeneratePInfo() // Antes del rank hay un 1, no sé por qué debe ser 1 para que detecte el personaje, cuando debe ser el CharacterId y no 1. Lo mismo con In.
        {
            var name = _session.Player.Name;
            var rank = _session.Player.Session.Account.Rank;
            var gender = _session.Player.Gender;
            var hairStyle = _session.Player.HairStyle;
            var hairColor = _session.Player.HairColor;
            var classId = _session.Player.Class;
            return $"c_info {name} - -1 -1 - 1 {(rank > 0 ? 6 : 0)} {(byte)gender} {(byte)hairStyle} {(byte)hairColor} {(byte)classId} 1 0 0 0 0 0 0";
        }
    }
}
