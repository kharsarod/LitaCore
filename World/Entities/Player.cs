using AutoMapper;
using Database.Player;
using Enum.Main.ChatEnum;
using Enum.Main.EntityEnum;
using Enum.Main.OptionEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Extensions;
using World.Network;

namespace World.Entities
{
    public class Player : Character
    {
        public ClientSession Session { get; set; }
        public short MapId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public byte Orientation { get; set; }
        public byte Scale { get; set; } = 100;
        public byte Speed { get; set; }
        public PlayerPackets Packets => new PlayerPackets(Session);
        public Character Character { get; set; }
        public bool IsSitting { get; set; }

        public Player(ClientSession session, Character character)
        {
            Session = session;
            Id = character.Id;
            AccountId = character.AccountId;
            Name = character.Name;
            Level = character.Level;
            Exp = character.Exp;
            JobLevel = character.JobLevel;
            JobExp = character.JobExp;
            HeroLevel = character.HeroLevel;
            HeroExp = character.HeroExp;
            Class = character.Class;
            Gender = character.Gender;
            HairColor = character.HairColor;
            HairStyle = character.HairStyle;
            Health = character.Health;
            MaxHealth = character.MaxHealth;
            Mana = character.Mana;
            MaxMana = character.MaxMana;
            Dignity = character.Dignity;
            Reputation = character.Reputation;
            Gold = character.Gold;
            Compliments = character.Compliments;
            MapId = character.MapId;
            MapPosX = character.MapPosX;
            MapPosY = character.MapPosY;
            Biography = character.Biography;
            Slot = character.Slot;
            Act4DeadCount = character.Act4DeadCount;
            Act4Victims = character.Act4Victims;
            Act4Points = character.Act4Points;
            IsArenaChampion = character.IsArenaChampion;
            IsBlockedBuff = character.IsBlockedBuff;
            IsEmoticonBlocked = character.IsEmoticonBlocked;
            IsExchangeBlocked = character.IsExchangeBlocked;
            IsFamilyRequestBlocked = character.IsFamilyRequestBlocked;
            IsFriendRequestBlocked = character.IsFriendRequestBlocked;
            IsGroupRequestBlocked = character.IsGroupRequestBlocked;
            IsHeroChatBlocked = character.IsHeroChatBlocked;
            IsHealthBlocked = character.IsHealthBlocked;
            MaxPets = character.MaxPets;
            IsMinilandInviteBlocked = character.IsMinilandInviteBlocked;
            MinilandMsg = character.MinilandMsg;
            MinilandPts = character.MinilandPts;
            MinilandState = character.MinilandState;
            CursorAimLock = character.CursorAimLock;
            IsQuickGetUpBlocked = character.IsQuickGetUpBlocked;
            RagePts = character.RagePts;
            SpecialistAddPts = character.SpecialistAddPts;
            SpecialistPts = character.SpecialistPts;
            State = character.State;
            TalentArenaLoses = character.TalentArenaLoses;
            TalentArenaSurrender = character.TalentArenaSurrender;
            TalentArenaWins = character.TalentArenaWins;
            IsWhispBlocked = character.IsWhispBlocked;
            IsDisplayHealthBlocked = character.IsDisplayHealthBlocked;
            IsDisplayCdBlocked = character.IsDisplayCdBlocked;
            IsBlockedHud = character.IsBlockedHud;
            IsBlockedHat = character.IsBlockedHat;
            IsPetAutoRelive = character.IsPetAutoRelive;
            IsPartnerAutoRelive = character.IsPartnerAutoRelive;
            Character = character;
        }

        public string GenerateIn()
        {
            // in 1 {session.Player.Name} - 532 123 876 2 1 3 45 2 654.321.543.678.987.345.234.123
            // 76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23"
            return Protocol.FormatPacket("in", (byte)Entity.Player, Name, "-",  Id, MapPosX, MapPosY, Orientation, Session.Account.Rank > 1 ? 2 : Session.Account.Rank,
                (byte)Gender, (byte)HairStyle, (byte)HairColor, 0, (byte)Class, "654.321.543.678.987.345.234.123", "76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23");
        }

        public async Task SetSpeed(byte speed)
        {
            Speed = speed;
            await Session.SendPacket(Packets.GeneratePlayerMove());
        }

        public async Task AddSpeed(byte speed)
        {
            Speed += speed;
            await Session.SendPacket(Packets.GeneratePlayerMove());
        }

        public async Task ChatSay(string message, ChatColor color)
        {
            // el segundo valor (1) es el charId.
            await Session.SendPacket($"say 1 1 {(byte)color} {message}");
        }

        public async Task UpdateCharacter()
        {
            await AppDbContext.UpdateAsync(Character);
        }
    }
}
