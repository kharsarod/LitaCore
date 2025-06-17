using Database.Helper;
using Database.Item;
using Database.Player;
using Enum.Main.BCardEnum;
using Enum.Main.BuffEnum;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using Enum.Main.EffectEnum;
using Enum.Main.EntityEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using Enum.Main.OptionEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public async Task<string> GenerateIn(int inEffect = 0)
        {
            var name = _session.Player.Name;
            var rank = _session.Player.Session.Account.Rank;
            var charId = _session.Player.Id;
            var gender = _session.Player.Gender;
            var hairStyle = _session.Player.HairStyle;
            var hairColor = _session.Player.HairColor;
            var classId = _session.Player.Class;
            var level = _session.Player.Level;
            var posX = _session.Player.MapPosX;
            var posY = _session.Player.MapPosY;
            var isInvisible = _session.Player.IsInvisible ? 1 : 0;
            var arenaWinner = _session.Player.IsArenaChampion ? 1 : 0;
            var compliments = _session.Player.Compliments;
            var size = _session.Player.Scale;
            var heroLevel = _session.Player.HeroLevel;
            var eqRarityPacket = await GenerateEqRarityAndUpgrade();
            var faction = 0;
            var orientation = _session.Player.Orientation;
            var repDignity = (_session.Player.GetDignityIcon() is 1 ? _session.Player.GetReputationIcon() : _session.Player.GetDignityIcon());
            string packet = $"in {(byte)Entity.Player} {name} - {charId} {posX} {posY} {orientation} {(rank > 0 ? 6 : 0)} {(byte)gender} {(byte)hairStyle} {(byte)hairColor} {(byte)classId} {await GenerateEqList()} {(_session.Player.Health / _session.Player.MaxHealth * 100)} {(_session.Player.Mana / _session.Player.MaxMana * 100)} {(_session.Player.IsSitting ? 1 : 0)} -1 0 0 0 0 {inEffect} {(_session.Player.UsingSpecialist ? _session.Player.Morph : _session.Player.IsUsingMount ? _session.Player.SpecialMorph : 0)} {eqRarityPacket} -1 - {repDignity} {isInvisible} {(_session.Player.UsingSpecialist ? _session.Player.SpUpgrade : 0)} {faction} {(_session.Player.UsingSpecialist ? (byte)_session.Player.SpWings : 0)} {level} 0 0|0|0 {arenaWinner} {compliments} {size} {heroLevel}";
            return packet;
        }

        public string GenerateGidx()
        {
            var characterId = _session.Player.Id;
            string packet = $"gidx 1 {characterId} -1 - 0 0|0|0";
            return packet;
        }

        public async Task GenerateInventoryExts()
        {
            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;
            packet.Append($"exts 0 {inventory.MAX_INVENTORY_SLOTS} {inventory.MAX_INVENTORY_SLOTS} {inventory.MAX_INVENTORY_SLOTS}");
            await _session.SendPacket(packet.ToString());
        }

        public string GenerateIcon(byte type, short value, short itemId)
        {
            return $"icon {type} {_session.Player.Id} {value} {itemId}";
        }

        public async Task GenerateSpAdditionPointsPacket()
        {
            StringBuilder packet = new StringBuilder();

            int maxAdditionPoints = 10000000; // 10KK.
            int maxSpPoint = 10000; // 10K base points.
            packet.Append($"sp {_session.Player.SpecialistAddPts} {maxAdditionPoints} {_session.Player.SpecialistPts} {maxSpPoint}");

            await _session.SendPacket(packet.ToString());
        }

        public async Task GenerateGoldPacket()
        {
            await _session.SendPacket($"gold {_session.Player.Gold}");
        }

        public async Task GenerateInvPacket()
        {
            var inventory = _session.Player.Inventory;

            if (inventory != null && inventory.Items != null)
            {
                var etcItems = inventory.Items.Where(x => x.InventoryType == InventoryType.ETC && x.Slot != -1).ToList();
                var mainItems = inventory.Items.Where(x => x.InventoryType == InventoryType.MAIN && x.Slot != -1).ToList();
                var wearItems = inventory.Items.Where(x => x.InventoryType == InventoryType.EQUIPMENT && x.Slot != -1).ToList();
                var spItems = inventory.Items.Where(x => x.InventoryType == InventoryType.SPECIALIST && x.Slot != -1).ToList();
                var costumeItems = inventory.Items.Where(x => x.InventoryType == InventoryType.COSTUME && x.Slot != -1).ToList();

                if (etcItems.Count > 0)
                {
                    string packet = $"{(byte)InventoryType.ETC}";
                    foreach (var item in etcItems)
                    {
                        packet += $" {item.Slot}.{item.ItemId}.{item.Amount}";
                    }
                    await _session.SendPacket($"inv {packet}");
                }
                else
                {
                    await _session.SendPacket($"inv {(byte)InventoryType.ETC}");
                }

                if (mainItems.Count > 0)
                {
                    string packet = $"{(byte)InventoryType.MAIN}";
                    foreach (var item in mainItems)
                    {
                        packet += $" {item.Slot}.{item.ItemId}.{item.Amount}";
                    }
                    await _session.SendPacket($"inv {packet}");
                }
                else
                {
                    await _session.SendPacket($"inv {(byte)InventoryType.MAIN}");
                }

                if (wearItems.Count > 0)
                {
                    string packet = $"{(byte)InventoryType.EQUIPMENT}";
                    foreach (var item in wearItems)
                    {
                        packet += $" {item.Slot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}.0.0.0.0";
                    }

                    await _session.SendPacket($"inv {packet}");
                }
                else
                {
                    await _session.SendPacket($"inv {(byte)InventoryType.EQUIPMENT}");
                }

                if (spItems.Count > 0)
                {
                    string packet = $"{(byte)InventoryType.SPECIALIST}";
                    foreach (var item in spItems)
                    {
                        packet += $" {item.Slot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}";
                    }
                    await _session.SendPacket($"inv {packet}");
                }
                else
                {
                    await _session.SendPacket($"inv {(byte)InventoryType.SPECIALIST}");
                }

                if (costumeItems.Count > 0)
                {
                    string packet = $"{(byte)InventoryType.COSTUME}";
                    foreach (var item in costumeItems)
                    {
                        packet += $" {item.Slot}.{item.ItemId}.{item.Amount}.0.0.0";
                    }
                    await _session.SendPacket($"inv {packet}");
                }
                else
                {
                    await _session.SendPacket($"inv {(byte)InventoryType.COSTUME}");
                }
            }

            await _session.Player.GetActionBarList(); // tener cuidado aquí por si la SP.
        }

        public async Task GenerateEquipmentPacket()
        {
            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;

            packet.Append($"equip");

            foreach (var item in inventory.Items.Where(x => x.InventoryType == InventoryType.WEAR))
            {
                // si es necesario agregar el IsColored ? item.Design : upgrade.
                // El último 0 es el runeAmount.
                packet.Append($" {(byte)item.Slot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}.0.0");
            }

            if (inventory.Items.Where(x => x.InventoryType == InventoryType.WEAR).Count() == 0)
            {
                await _session.SendPacket("equip 0 0");
                return;
            }

            await _session.SendPacket(packet.ToString());
            await _session.Player.CurrentMap.Broadcast(await GenerateEqPacket());
        }

        public async Task GeneratePairyPacket()
        {
            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;
            packet.Append("pairy 1");
            var valueAfterCharId = 4;

            var fairy = inventory.GetEquippedItemFromSlot((byte)EquipmentType.FAIRY);
            if (fairy is null) return;
            var getItem = WorldManager.GetItem(fairy.ItemId);

            packet.Append($" {_session.Player.Id} {valueAfterCharId} {getItem.Element} {getItem.ElementRate} {getItem.Model}");

            await _session.Player.CurrentMap.Broadcast(packet.ToString());
        }

        public async Task GeneratePlayersFairy()
        {
            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;
            var fairy = inventory.GetEquippedItemFromSlot((byte)EquipmentType.FAIRY);
            if (fairy is null) return;
            var getItem = WorldManager.GetItem(fairy.ItemId);
            packet.Append("pairy 1");

            foreach (var player in _session.Player.CurrentMap.Players.Where(x => _session.Player.Id != x.Id))
            {
                packet.Append($" {player.Id} {player.Id} {getItem.Element} {getItem.ElementRate} {getItem.Model}");
                await _session.SendPacket(packet.ToString());
                packet.Clear();
            }
        }

        public async Task GenerateRsfpPacket()
        {
            await _session.SendPacket($"rsfp 0 -1");
        }

        public async Task GeneratePinitPacket()
        {
            await _session.SendPacket("pinit 0");
        }

        public async Task<string> GenerateEqPacket()
        {
            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;
            var isGm = _session.Account.Rank > 0 ? 6 : 0;
            var gender = _session.Player.Gender;
            var hairStyle = _session.Player.HairStyle;
            var hairColor = _session.Player.HairColor; // El IsColored es para el color de cabello creo.
            var profession = _session.Player.Class;
            var eqListPacket = await GenerateEqList();

            packet.Append($"eq {_session.Player.Id} {isGm} {(byte)gender} {(byte)hairStyle} {(byte)hairColor} {(byte)profession} {eqListPacket} {await GenerateEqRarityAndUpgrade()}");

            return packet.ToString();
        }

        public async Task<string> GenerateEqRarityAndUpgrade()
        {
            var mainWeapon = _session.Player.Inventory.GetEquippedItemFromSlot((byte)EquipmentType.MAIN_WEAPON);
            var armor = _session.Player.Inventory.GetEquippedItemFromSlot((byte)EquipmentType.ARMOR);
            return $"{mainWeapon?.Upgrade ?? 0}{mainWeapon?.Rarity ?? 0} {armor?.Upgrade ?? 0}{armor?.Rarity ?? 0}";
        }

        public async Task<string> GenerateEqList()
        {
            var inventory = _session.Player.Inventory;
            var hasDisabledHat = _session.Player.IsBlockedHat;
            var invTasks = Enumerable.Range(0, 17)
                .Select(async slot =>
                {
                    var item = await inventory.GetItemFromSlot(slot, InventoryType.WEAR);
                    return item?.ItemId.ToString() ?? "-1";
                })
                .ToArray();

            var invArray = await Task.WhenAll(invTasks);

            string hatVNum = hasDisabledHat ? "0" : invArray[(byte)EquipmentType.HAT];

            var packetValues = new[]
            {
                hatVNum,
                invArray[(byte)EquipmentType.ARMOR],
                invArray[(byte)EquipmentType.MAIN_WEAPON],
                invArray[(byte)EquipmentType.SECONDARY_WEAPON],
                invArray[(byte)EquipmentType.MASK],
                invArray[(byte)EquipmentType.FAIRY],
                invArray[(byte)EquipmentType.COSTUME_SUIT],
                invArray[(byte)EquipmentType.COSTUME_HAT],
                invArray[(byte)EquipmentType.WEAPON_SKIN],
                invArray[(byte)EquipmentType.WINGS],
                "-1"
            };
            return string.Join(".", packetValues);
        }

        public async Task GenerateScrollingMessagesPacket()
        {
            StringBuilder packet = new StringBuilder();
            var messages = await AuthDbHelper.LoadAllScrollingMessagesAsync();
            for (byte i = 0; i < messages.Count; i++)
            {
                packet.Append($"bn {i} {messages[i].Message.Replace(' ', '^')}");
                await _session.SendPacket(packet.ToString());
                packet.Clear();
            }
        }

        public string GenerateLev()
        {
            var level = _session.Player.Level;
            var exp = level < 100 ? _session.Player.Exp : _session.Player.Exp / 100;
            var jobLevel = _session.Player.JobLevel;
            var jobExp = jobLevel < 81 ? _session.Player.JobExp : _session.Player.JobExp / 100;
            var heroLevel = _session.Player.HeroLevel;
            var heroExp = heroLevel <= 99 ? _session.Player.HeroExp : _session.Player.HeroExp / 100;

            return $"lev {level} {exp} {jobLevel} {jobExp} {_session.Player.Stats.ExperienceToUp} {_session.Player.Stats.JobExperienceToUp} 10 15 {heroExp} {heroLevel} 15000 0";
        }

        public string GenerateMove(short walkPacketX, short walkPacketY)
        {
            return $"mv {(byte)Entity.Player} {_session.Player.Id} {walkPacketX} {walkPacketY} {_session.Player.Speed}";
        }

        /// <summary>
        /// c_map packet
        /// </summary>
        /// <returns></returns>
        public string GenerateMapInfo()
        {
            return $"c_map 0 {_session.Player.CurrentMap.Id} 1";
        }

        public string GenerateEffect(Effect effect)
        {
            return $"eff 1 {_session.Player.Id} {(byte)effect}";
        }

        public string GenerateEffectPacket(int effectId)
        {
            return $"eff 1 {_session.Player.Id} {effectId}";
        }

        /// <summary>
        /// sc packet
        /// </summary>
        /// <returns></returns>
        public async Task GenerateScPacket()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append("sc");

            var mainWeapon = _session.Player.Inventory.GetEquippedItemFromSlot((byte)EquipmentType.MAIN_WEAPON);
            var armor = _session.Player.Inventory.GetEquippedItemFromSlot((byte)EquipmentType.ARMOR);
            var secondWeapon = _session.Player.Inventory.GetEquippedItemFromSlot((byte)EquipmentType.SECONDARY_WEAPON);

            // Valores por defecto
            int minDmg = 0, maxDmg = 0, hitRate = 0, critChance = 0, critRate = 0;
            int minDmgSecond = 0, maxDmgSecond = 0, hitRateSecond = 0, critChanceSecond = 0, critRateSecond = 0;
            int closeDefence = 0, distanceDefence = 0, magicDefence = 0, dodge = 0;
            byte mainUpgrade = 0, secondUpgrade = 0, armorUpgrade = 0;

            if (mainWeapon != null)
            {
                var getItemMainWeapon = WorldManager.GetItem(mainWeapon.ItemId);
                if (getItemMainWeapon != null)
                {
                    minDmg = getItemMainWeapon.DamageMinimum + mainWeapon.MinDmg;
                    maxDmg = getItemMainWeapon.DamageMaximum + mainWeapon.MaxDmg;
                    hitRate = getItemMainWeapon.HitRate + mainWeapon.HitRate;
                    critChance = getItemMainWeapon.CriticalLuckRate + mainWeapon.CritLuckRate;
                    critRate = getItemMainWeapon.CriticalRate + mainWeapon.CritRate;
                    mainUpgrade = mainWeapon.Upgrade;
                }
            }

            if (secondWeapon != null)
            {
                var getItemSecondWeapon = WorldManager.GetItem(secondWeapon.ItemId);
                if (getItemSecondWeapon != null)
                {
                    minDmgSecond = getItemSecondWeapon.DamageMinimum + secondWeapon.MinDmg;
                    maxDmgSecond = getItemSecondWeapon.DamageMaximum + secondWeapon.MaxDmg;
                    hitRateSecond = getItemSecondWeapon.HitRate + secondWeapon.HitRate;
                    critChanceSecond = getItemSecondWeapon.CriticalLuckRate + secondWeapon.CritLuckRate;
                    critRateSecond = getItemSecondWeapon.CriticalRate + secondWeapon.CritRate;
                    secondUpgrade = secondWeapon.Upgrade;
                }
            }

            if (armor != null)
            {
                var getItemArmor = WorldManager.GetItem(armor.ItemId);
                if (getItemArmor != null)
                {
                    closeDefence = getItemArmor.CloseDefence + armor.CloseDefence;
                    distanceDefence = getItemArmor.DistanceDefence + armor.DistDefence;
                    magicDefence = getItemArmor.MagicDefence + armor.MagicDefence;
                    dodge = getItemArmor.DefenceDodge + armor.DefDodge;
                    armorUpgrade = armor.Upgrade;
                }
            }

            var firstType = _session.Player.IsSwordsman
                || _session.Player.IsAdventurer
                || _session.Player.IsMartialArtist ? 0
                : _session.Player.IsMage ? 2 : 1;

            var secondType = _session.Player.IsSwordsman
                || _session.Player.IsAdventurer
                || _session.Player.IsMartialArtist ? 0
                : 1;

            packet.Append($" {firstType} {mainUpgrade} {minDmg} {maxDmg} {hitRate} {critChance} {critRate} {secondType} {secondUpgrade}" +
                $" {minDmgSecond} {maxDmgSecond} {hitRateSecond} {critChanceSecond} {critRateSecond} {armorUpgrade} {closeDefence} {dodge} {distanceDefence} {dodge} {magicDefence}");

            await _session.SendPacket(packet.ToString());
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

            return $"stat {player.Stats.CurrentHealth} {player.Stats.MaxHealth().Result} {player.Stats.CurrentMana} {player.Stats.MaxMana().Result} 0 {option}";
        }

        public async Task GenerateHealPacket(int heal)
        {
            string packet = $"rc 1 {_session.Player.Id} {heal} -1";
            await _session.SendPacket(packet);
        }

        public async Task GenerateGetDamagePacket(int amount)
        {
            string packet = $"dm 1 {_session.Player.Id} {amount} 1";
            await _session.SendPacket(packet);
        }

        public string GeneratePlayerMove()
        {
            var speed = _session.Player.Speed;
            var charId = _session.Player.Id;

            bool hasNoMoveBuff = false;
            bool hasNoAttackBuff = false;

            foreach(var buff in _session.Player.Buffs.Where(x => x.BuffType == BuffType.Bad))
            {
                var bCards = WorldManager.GetBCardsFromBuff(buff.BuffId).Result;
                foreach(var bCard in bCards)
                {
                    if (bCard.Type == BCardType.Movement && bCard.SubType == (BCardEffect)11 && bCard.FirstEffectValue == 0)
                    {
                        hasNoMoveBuff = true;
                    }
                    if (bCard.Type == BCardType.SPECIAL_ATTACK && bCard.SubType == (BCardEffect)11 && bCard.FirstEffectValue == 0)
                    {
                        hasNoAttackBuff = true;
                    }
                }
            }

            return $"cond 1 {charId} {(hasNoAttackBuff ? 1 : 0)} {(hasNoMoveBuff ? 1 : 0)} {speed}";
        }

        /// <summary>
        /// at packet
        /// </summary>
        /// <returns></returns>
        public async Task<string> GeneratePlayerMapInfo()
        {
            var instance = await WorldManager.GetInstance(_session.Player.MapId, _session.ChannelId);
            var charId = _session.Player.Id;
            return $"at {charId} {instance.Id} {_session.Player.MapPosX} {_session.Player.MapPosY} {_session.Player.Orientation} 0 {instance.Template.Bgm} 1 -1";
        }

        public async Task GenerateRsfiPacket()
        {
            string packet = "rsfi 1 6 0 5 0 5";
            await _session.SendPacket(packet);
        }

        public async Task GenerateScrPacket()
        {
            string packet = "scr 0 0 0 0 0 0";
            await _session.SendPacket(packet);
        }

        public string GenerateRage()
        {
            return $"rage 0 0";
        }

        public async Task GenerateClosePacket(byte value = 0)
        {
            await _session.SendPacket($"c_close {value}");
        }

        public async Task GenerateFStashEndPacket()
        {
            await _session.SendPacket($"f_stash_end");
        }

        public async Task GenerateMall(byte value)
        {
            await _session.SendPacket($"mall {value}");
        }

        public string GenerateScale()
        {
            var scale = _session.Player.Scale;
            var charId = _session.Player.Id;
            return $"char_sc 1 {charId} {scale}";
        }

        public string GenerateFood()
        {
            return "food 0";
        }

        public string GenerateClientAuthPacket()
        {
            var charId = _session.Player.Id;
            var userName = _session.Player.Session.Account.Username;
            var name = _session.Player.Name;

            return $"twk 10 {charId} {userName} {name} shtmxpdlfeoqkr en es";
        }

        public async Task GenerateRepAndDignityPacket()
        {
            StringBuilder packet = new StringBuilder();
            var reputation = _session.Player.Reputation;
            var dignity = _session.Player.Dignity;
            packet.Append($"fd {reputation} {_session.Player.GetReputationIcon()} {(int)dignity} {Math.Abs(_session.Player.GetDignityIcon())}");

            await _session.SendPacket(packet.ToString());
            await _session.Player.CurrentMap.Broadcast(await GenerateIn(1));
            await _session.Player.CurrentMap.Broadcast(GenerateGidx());
        }

        public async Task MapOutPacket()
        {
            await _session.SendPacket("mapout");
        }

        public async Task OutPacket()
        {
            await _session.Player.CurrentMap.Broadcast($"out 1 {_session.Player.Id}", _session);
        }

        public async Task GenerateCModePacket()
        {
            // c_mode 1 25459 2 5 0 0 100 0

            StringBuilder packet = new StringBuilder();
            var inventory = _session.Player.Inventory;
            if (inventory is null) return;

            var getItem = inventory.GetEquippedItemFromSlot((int)EquipmentType.SPECIALIST);

            var morph = 0;

            if (_session.Player.IsSpTransformed)
            {
                morph = _session.Player.Morph;

                if (_session.Player.IsMountTransformed)
                {
                    morph = _session.Player.SpecialMorph;
                }
            }
            else if (!_session.Player.IsSpTransformed)
            {
                if (_session.Player.IsMountTransformed)
                {
                    morph = _session.Player.SpecialMorph;
                }
                else if (_session.Player.IsSpTransformed)
                {
                    morph = _session.Player.Morph;
                }
            }

            var isMountTransformed = _session.Player.IsMountTransformed;

            packet.Append($"c_mode 1 {_session.Player.Id} {morph} {(getItem is null ? 0 : isMountTransformed ? 0 : getItem.Upgrade)} {(getItem is null ? 0 : getItem.Rarity)} 0 100 0");

            // 0 después de upgrade es un arenaWinner, los demás no sé.

            await _session.Player.CurrentMap.Broadcast(packet.ToString());
        }

        public string GenerateStatInfo()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append("st 1");

            var charId = _session.Player.Id;
            var level = _session.Player.Level;
            var heroLevel = _session.Player.HeroLevel;

            packet.Append($" {charId} {level} {heroLevel} 100 100 -1");

            return packet.ToString();
        }

        public async Task GenerateEffect(int effectId)
        {
            string packet = $"eff 1 {_session.Player.Id} {effectId}";
            await _session.Player.CurrentMap.Broadcast(packet);
        }

        /// <summary>
        /// <paramref name="data3"/> Puede ser itemId que se usará al momento de responder o puede ser otro dato.<para/>
        /// Envía una pregunta al jugador.<para/> Si el parámetro <paramref name="isItem"/> es true, entonces <paramref name="itemId"/> debe tener un valor, de lo contrario 0.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <param name="message"></param>
        /// <param name="isItem"></param>
        /// <param name="itemId"></param>
        /// <param name="unknownData"></param>
        /// <returns></returns>
        public async Task SendQNaiAsk(short data1, short data2, short data3, MessageId message, bool isItem = false, short itemId = 0, short unknownData = 0)
        {
            var _isItem = isItem ? 1 : 0;
            StringBuilder packet = new StringBuilder();
            packet.Append($"qnai #guri^{data1}^{data2}^{data3} {(short)message} {_isItem} {itemId} {unknownData}");
            await _session.SendPacket(packet.ToString());
            packet.Clear();
        }

        /// <summary>
        /// c_info packet
        /// </summary>
        /// <returns></returns>
        public string GeneratePInfo()
        {
            var name = _session.Player.Name;
            var rank = _session.Player.Session.Account.Rank;
            var gender = _session.Player.Gender;
            var hairStyle = _session.Player.HairStyle;
            var hairColor = _session.Player.HairColor;
            var classId = _session.Player.Class;
            var charId = _session.Player.Id;
            var dig = _session.Player.GetDignityIcon();
            var rep = _session.Player.GetReputationIcon();
            var useMorph = _session.Player.UsingSpecialist || _session.Player.IsUsingMount ? _session.Player.Morph : 0;
            var useSp = _session.Player.UsingSpecialist ? _session.Player.SpWings : 0;
            return $"c_info {name} - -1 -1 - {charId} {(rank > 0 ? 6 : 0)} {(byte)gender} {(byte)hairStyle} {(byte)hairColor} {(byte)classId} {(rep == 1 ? dig : -dig)} 0 {useMorph} 0 0 {useSp} 0";
        }
    }
}