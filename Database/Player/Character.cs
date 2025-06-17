using Enum.Main.CharacterEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Player
{
    public class Character
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? Name { get; set; }
        public byte Level { get; set; }
        public long Exp { get; set; }
        public byte JobLevel { get; set; }
        public long JobExp { get; set; }
        public byte HeroLevel { get; set; }
        public long HeroExp { get; set; }
        public ClassId Class { get; set; }
        public Gender Gender { get; set; }
        public HairColor HairColor { get; set; }
        public HairStyle HairStyle { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public float Dignity { get; set; }
        public int Reputation { get; set; }
        public long Gold { get; set; }
        public short Compliments { get; set; }
        public short MapId { get; set; }
        public short MapPosX { get; set; }
        public short MapPosY { get; set; }
        public string? Biography { get; set; }
        public byte Slot { get; set; }
        public int Act4DeadCount { get; set; }
        public int Act4Victims { get; set; }
        public int Act4Points { get; set; }
        public bool IsArenaChampion { get; set; }
        public bool IsBlockedBuff { get; set; }
        public bool IsEmoticonBlocked { get; set; }
        public bool IsExchangeBlocked { get; set; }
        public bool IsFamilyRequestBlocked { get; set; }
        public bool IsFriendRequestBlocked { get; set; }
        public bool IsGroupRequestBlocked { get; set; }
        public bool IsHeroChatBlocked { get; set; }
        public bool IsHealthBlocked { get; set; }
        public byte MaxPets { get; set; }
        public bool IsMinilandInviteBlocked { get; set; }
        public string? MinilandMsg { get; set; }
        public short MinilandPts { get; set; }
        public bool MinilandState { get; set; }
        public bool CursorAimLock { get; set; }
        public bool IsQuickGetUpBlocked { get; set; }
        public long RagePts { get; set; }
        public int SpecialistAddPts { get; set; }
        public int SpecialistPts { get; set; }
        public byte State { get; set; }
        public int TalentArenaLoses { get; set; }
        public int TalentArenaSurrender { get; set; }
        public int TalentArenaWins { get; set; }
        public bool IsWhispBlocked { get; set; }
        public bool IsDisplayHealthBlocked { get; set; }
        public bool IsDisplayCdBlocked { get; set; }
        public bool IsBlockedHud { get; set; }
        public bool IsBlockedHat { get; set; }
        public bool IsPetAutoRelive { get; set; }
        public bool IsPartnerAutoRelive { get; set; }
        public short ServerId { get; set; }
    }
}