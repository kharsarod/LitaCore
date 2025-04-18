﻿using Enum.Main.CharacterEnum;
using Enum.Main.EffectEnum;
using Enum.Main.EntityEnum;
using Enum.Main.OptionEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using World.Entities;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Extensions
{
    public class PlayerPackets
    {
        private ClientSession _session;
        public PlayerPackets(ClientSession session)
        {
            _session = session;
        }

        public string GenerateIn()
        {
            // in 1 {session.Player.Name} - 532 123 876 2 1 3 45 2 654.321.543.678.987.345.234.123
            // 76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23"
            
            var name = _session.Player.Name;
            var rank = _session.Player.Session.Account.Rank;
            var charId = _session.Player.Character.Id;
            var gender = _session.Player.Gender;
            var hairStyle = _session.Player.HairStyle;
            var hairColor = _session.Player.HairColor;
            var classId = _session.Player.Class;
            var level = _session.Player.Character.Level;
            var posX = _session.Player.MapPosX;
            var posY = _session.Player.MapPosY;


            return $"in {(byte)Entity.Player} {name} - {charId} {posX} {posY} 1 {(rank > 0 ? 6 : 0)} {(byte)gender} {(byte)hairStyle} {(byte)hairColor} {(byte)classId} 654.321.543.678.987.345.234.123 100 100 0 0 0 0 0 0 0 {(_session.Player.UsingSpecialist ? _session.Player.Morph : 0)} 0 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 {(_session.Player.UsingSpecialist ? _session.Player.SpUpgrade : 0)} 1 {(_session.Player.UsingSpecialist ? (byte)_session.Player.SpWings : 0)} 45 12 0|0|1  1 99 100 23";
        }

        public string GenerateLev()
        {
            var level = _session.Player.Character.Level;
            var exp = level <= 99 ? _session.Player.Character.Exp : _session.Player.Exp / 100;
            var jobLevel = _session.Player.Character.JobLevel;
            var jobExp = _session.Player.Character.JobExp;
            var heroLevel = _session.Player.Character.HeroLevel;
            var heroExp = heroLevel <= 99 ? _session.Player.HeroExp : _session.Player.HeroExp / 100;

            return $"lev {level} {exp} {jobLevel} {jobExp} 1000 1500 10 15 {heroExp} {heroLevel} 15000 0";
        }

        public string GenerateMove(short walkPacketX, short walkPacketY)
        {
            return $"mv {(byte)Entity.Player} {_session.Player.Character.Id} {walkPacketX} {walkPacketY} {_session.Player.Speed}";
        }

        public string GenerateMapInfo()
        {
            return $"c_map 0 {_session.Player.CurrentMap.Id} 0";
        }

        public string GenerateEffect(Effect effect)
        {
            return $"eff 1 1 {(byte)effect}";
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

        public string GeneratePlayerMapInfo()
        {
            var instance = WorldManager.GetInstance(_session.Player.Character.MapId);
            return $"at 1 {instance.Id} {_session.Player.Character.MapPosX} {_session.Player.Character.MapPosY} 0 0 {instance.Template.Bgm} 2 -1";
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
