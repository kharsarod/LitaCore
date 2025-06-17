using Configuration;
using Database.Helper;
using Database.Player;
using Database.World;
using Enum.Main.CharacterEnum;
using Enum.Main.ItemEnum;
using GameWorld;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay;
using World.GameWorld;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class CharacterHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("Char_NEW", HandleCharacterCreation);
            handler.Register("ORG", HandleCharacterLoad);
            handler.Register("select", HandleSelect);
            handler.Register("Char_DEL", HandleCharacterDelete);
            handler.Register("Char_NEW_JOB", HandleCharacterCreationJob);
        }

        public static async Task HandleCharacterCreation(ClientSession session, string[] parts) // Cambiar HandleCharacter -> HandleCharacterCreation
        {
            await session.SendPacket("clist_start 1");

            Character character = new Character
            {
                AccountId = session.Account.Id,
                Name = parts[2],
                Level = ConfigManager.WorldServerConfig.Gameplay.StartPlayerLevel,
                JobLevel = ConfigManager.WorldServerConfig.Gameplay.StartPlayerJobLevel,
                HeroExp = 0,
                HeroLevel = 0,
                Reputation = ConfigManager.WorldServerConfig.Gameplay.StartReputation,
                MapId = ConfigManager.WorldServerConfig.Gameplay.StartPlayerMapID,
                MapPosX = ConfigManager.WorldServerConfig.Gameplay.StartPlayerPositionX,
                MapPosY = ConfigManager.WorldServerConfig.Gameplay.StartPlayerPositionY,
                MaxHealth = 221,
                Health = 221,
                MaxMana = 350,
                Mana = 350,
                Dignity = ConfigManager.WorldServerConfig.Gameplay.StartDignity, // cargar cuenta con el loadByName, ya que al entrar sale el NOS y sale el nombre de la cuenta, sino es el NOS es el NsTeST.
                Slot = byte.Parse(parts[3]),
                Gender = (Gender)byte.Parse(parts[4]),
                HairStyle = (HairStyle)byte.Parse(parts[5]),
                HairColor = (HairColor)byte.Parse(parts[6]),
                Class = ClassId.Adventurer,
                Gold = ConfigManager.WorldServerConfig.Gameplay.StartGold,
                ServerId = WorldManager.GetServerId()
            };
            Log.Information($"Character created: {character.Name} - session: {session.Account.Id} - {session.Account.Username}");
            await session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} 0.0.0.0.0.0.0.0 0 0 -1 -1 -1");
            await session.SendPacket("clist_end");

            WorldLog worldLog = new WorldLog
            {
                Id = 0, // Assuming Id is auto-incremented in the database
                Source = character.Name,
                Timestamp = DateTime.Now,
                Type = "NewCharacter",
            };
            await WorldDbHelper.InsertWorldLogAsync(worldLog);

            try
            {
                await CharacterDbHelper.InsertAsync(character);
                List<CharacterSkill> insertSkills= new List<CharacterSkill>
                {
                    new CharacterSkill
                    {
                        CharacterId = character.Id,
                        VNum = 200,
                        CastId = (byte)WorldManager.GetCastIdOfSkill(200),
                    },
                    new CharacterSkill
                    {
                        CharacterId = character.Id,
                        VNum = 201,
                        CastId = (byte)WorldManager.GetCastIdOfSkill(201),
                    },
                    new CharacterSkill
                    {
                        CharacterId = character.Id,
                        VNum = 209,
                        CastId = (byte)WorldManager.GetCastIdOfSkill(202),
                    }
                };
                var inv = new Inventory(session, character.Id);
                inv.CharacterId = character.Id;
                await inv.AddItemToInventory(character.Id, 1, 1, 0, 0);
                await inv.AddItemToInventory(character.Id, 12, 1, 0, 0);
                await inv.AddItemToInventory(character.Id, 8, 1, 0, 0);
                await inv.AddItemToInventory(character.Id, 2024, 10, 0, 0);
                await inv.AddItemToInventory(character.Id, 2081, 5, 0, 0);

                List<ActionBar> insertActionBars = new List<ActionBar>
                {
                    new ActionBar // Sit
                    {
                        CharacterId = character.Id,
                        Morph = 0,
                        Slot = 3,
                        Pos = 1,
                        Q1 = 0,
                        Q2 = 9,
                        Type = 1
                    },
                    new ActionBar
                    {
                        CharacterId = character.Id,
                        Morph = 0,
                        Slot = 1,
                        Pos = 0,
                        Q1 = 0,
                        Q2 = 0,
                        Type = 1
                    },
                    new ActionBar
                    {
                        CharacterId = character.Id,
                        Slot = 1,
                        Morph = 0,
                        Pos = 1,
                        Q1 = 0,
                        Q2 = 1,
                        Type = 1,
                    },
                    new ActionBar
                    {
                        CharacterId = character.Id,
                        Slot = 1,
                        Morph = 0,
                        Pos = 16,
                        Q1 = 0,
                        Q2 = 13,
                        Type = 1
                    }
                };


                await inv.EquipAdventurerItems();
                await CharacterDbHelper.InsertCharacterSkills(insertSkills);
                await CharacterDbHelper.InsertActionBarListAsync(insertActionBars);
                await HandleCharacterLoad(session, parts);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while saving character: {ex.Message} - InnerException: {ex.InnerException?.Message}");
            }
        }

        public static async Task HandleCharacterCreationJob(ClientSession session, string[] parts)
        {
            await session.SendPacket("clist_start 1");
            var classByte = byte.Parse(parts[7]);
            Character character = new Character
            {
                AccountId = session.Account.Id,
                Name = parts[2],
                Level = 56,
                JobLevel = 20,
                HeroExp = 0,
                HeroLevel = 0,
                Reputation = 10000,
                MapId = 1,
                MapPosX = 79,
                MapPosY = 116,
                MaxHealth = 5000,
                Health = 5000,
                MaxMana = 7000,
                Mana = 7000,
                Dignity = 0, // cargar cuenta con el loadByName, ya que al entrar sale el NOS y sale el nombre de la cuenta, sino es el NOS es el NsTeST.
                Slot = byte.Parse(parts[3]),
                Gender = (Gender)byte.Parse(parts[4]),
                HairStyle = (HairStyle)byte.Parse(parts[5]),
                HairColor = (HairColor)byte.Parse(parts[6]),
                Class = (ClassId)(byte)(classByte == 2 ? (byte)ClassId.Swordsman : classByte == 3 ? (byte)ClassId.Archer : classByte == 4 ? (byte)ClassId.Mage : 1),
                Gold = 0,
                ServerId = WorldManager.GetServerId()
            };
            Log.Information($"Character created: {character.Name} - session: {session.Account.Id} - {session.Account.Username}");
            await session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} 0.0.0.0.0.0.0.0 0 0 -1 -1 -1");
            await session.SendPacket("clist_end");

            try
            {
                await CharacterDbHelper.InsertAsync(character);
                await HandleCharacterLoad(session, parts);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while saving character: {ex.Message} - InnerException: {ex.InnerException?.Message}");
            }
        }

        public static async Task HandleCharacterLoad(ClientSession session, string[] parts)
        {
            try
            {
                byte isNosFireEventEnabled = 0;
                await session.SendPacket($"clist_start {isNosFireEventEnabled}");
                foreach (var character in await CharacterDbHelper.LoadCharactersByServerIdAndAccountId(WorldManager.GetServerId(), session.Account.Id))
                {
                    var items = await CharacterDbHelper.LoadCharacterItemsByCharacterId(character.Id);
                    var armorItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.ARMOR && i.InventoryType == InventoryType.WEAR);
                    var mainWeaponItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.MAIN_WEAPON && i.InventoryType == InventoryType.WEAR);
                    var secondaryWeaponItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.SECONDARY_WEAPON && i.InventoryType == InventoryType.WEAR);
                    var hatItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.HAT && i.InventoryType == InventoryType.WEAR);
                    var maskItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.MASK && i.InventoryType == InventoryType.WEAR);
                    var costumeItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.COSTUME_SUIT && i.InventoryType == InventoryType.WEAR);
                    var costumeHatItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.COSTUME_HAT && i.InventoryType == InventoryType.WEAR);

                    var skinItem = items.FirstOrDefault(i => i.Slot == (byte)EquipmentType.WEAPON_SKIN && i.ItemId != 0 && i.InventoryType == InventoryType.WEAR);

                    string packet = $"clist {character.Slot} {character.Name} 0 " +
                                    $"{(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 " +
                                    $"{(byte)character.Class} {character.Level} {character.HeroLevel} " +
                                    $"{hatItem?.ItemId ?? -1}.{armorItem?.ItemId ?? -1}.{skinItem?.ItemId ?? mainWeaponItem?.ItemId ?? -1}.{secondaryWeaponItem?.ItemId ?? 0}.{maskItem?.ItemId ?? 0}.-1." +
                                    $"{costumeItem?.ItemId ?? 0}.{costumeHatItem?.ItemId ?? 0}.-1.0 0 0 -1 -1 -1";

                    await session.SendPacket(packet);
                }

                await session.SendPacket("clist_end");
            }
            catch (Exception e)
            {
                Log.Warning("Error while loading " + " " + e.Message);
            }
        }

        public static async Task HandleCharacterDelete(ClientSession session, string[] parts)
        {
            try
            {
                if (Cryptography.ToSha512(parts[3]) == session.Account.Password)
                {
                    await session.SendPacket("clist_start 0");
                    var character = await CharacterDbHelper.LoadCharacterBySlotAndAccountId(byte.Parse(parts[2]), session.Account.Id, WorldManager.GetServerId());
                    var logs = await WorldDbHelper.LoadWorldLogsBySourceAsync(character.Name);
                    foreach (var log in logs)
                    {
                        await WorldDbHelper.RemoveWorldLogBySource(log.Source);
                    }
                    await CharacterDbHelper.DeleteCharacterAsync(character);
                    await HandleCharacterLoad(session, parts);
                    await session.SendPacket("clist_end");
                }
                else
                {
                    await session.SendPacket($"msg 0 Password doesn't match.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Error while deleting " + " " + ex.Message);
            }
        }

        public static async Task HandleSelect(ClientSession session, string[] parts) // Cambiar Select -> HandleSelect
        {
            try
            {
                var character = await CharacterDbHelper.LoadCharacterBySlotAndAccountId(byte.Parse(parts[2]), session.Account.Id, WorldManager.GetServerId());
                session.Player = new Player(session, character);
                Console.WriteLine($"Map: {character.Name}");
                var map = await WorldManager.GetInstance(character.MapId, session.ChannelId);
                session.Player.CurrentMap = map;
                await session.SendPacket("OK");
            }
            catch (Exception e)
            {
                Log.Warning("ERROR: " + e.Message);
            }
        }
    }
}