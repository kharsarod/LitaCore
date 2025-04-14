using Database.Player;
using Enum.Main.CharacterEnum;
using GameWorld;
using NosCryptLib.Encryption;
using NosTalePacketsLib.Packets.Client;
using NosTalePacketsLib.Packets.Parser;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.GameWorld;

namespace World.Network.Handlers
{
    public class CharacterHandler
    {
        public static async Task HandleCharacterCreation(ClientSession session, string[] parts) // Cambiar HandleCharacter -> HandleCharacterCreation
        {
            await session.SendPacket("clist_start 0");

            Character character = new Character
            {
                AccountId = session.Account.Id,
                Name = parts[2],
                Level = 1,
                JobLevel = 1,
                HeroExp = 0,
                HeroLevel = 0,
                Reputation = 0,
                MapId = 1,
                MapPosX = 79,
                MapPosY = 116,
                MaxHealth = 221,
                Health = 221,
                MaxMana = 350,
                Mana = 350,
                Dignity = 0, // cargar cuenta con el loadByName, ya que al entrar sale el NOS y sale el nombre de la cuenta, sino es el NOS es el NsTeST.
                Slot = byte.Parse(parts[3]),
                Gender = (Gender)byte.Parse(parts[4]),
                HairStyle = (HairStyle)byte.Parse(parts[5]),
                HairColor = (HairColor)byte.Parse(parts[6]),
                Class = ClassId.Adventurer,
                Gold = 0
            };
            Log.Information($"Character created: {character.Name} - session: {session.Account.Id} - {session.Account.Username}");
            await session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} 0.0.0.0.0.0.0.0 0 0 -1 -1 -1");
            await session.SendPacket("clist_end");

            try
            {
                await AppDbContext.InsertAsync(character);
                await HandleCharacterLoad(session, parts);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while saving character: {ex.Message} - InnerException: {ex.InnerException?.Message}");
            }

        }

        public static async Task HandleCharacterCreationJob(ClientSession session, string[] parts)
        {
            await session.SendPacket("clist_start 0");
            var classByte = byte.Parse(parts[7]);
            Character character = new Character
            {
                AccountId = session.Account.Id,
                Name = parts[2],
                Level = 56,
                JobLevel = 20,
                HeroExp = 0,
                HeroLevel = 0,
                Reputation = 0,
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
                Gold = 0
            };
            Log.Information($"Character created: {character.Name} - session: {session.Account.Id} - {session.Account.Username}");
            await session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} 0.0.0.0.0.0.0.0 0 0 -1 -1 -1");
            await session.SendPacket("clist_end");

            try
            {
                await AppDbContext.InsertAsync(character);
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
                await session.SendPacket("clist_start 0");
                foreach(var character in await AppDbContext.LoadCharactersByAccountId(session.Account.Id))
                {
                    await session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} 0.0.0.0.0.0.0.0 0 0 -1 -1 -1");
                    
                }
                await session.SendPacket("clist_end");
            }
            catch (Exception e)
            {
                Log.Warning("Error while loading character." + " " + e.Message);
            }
        }

        public static async Task HandleCharacterDelete(ClientSession session, string[] parts)
        {
            try
            {
                if (Cryptography.ToSha512(parts[3]) == session.Account.Password)
                {
                    await session.SendPacket("clist_start 0");
                    var character = await AppDbContext.LoadCharacterBySlot(byte.Parse(parts[2]));
                    await AppDbContext.DeleteCharacterAsync(character);
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
                Log.Warning("Error while deleting character." + " " + ex.Message);
            }
        }

        public static async Task HandleSelect(ClientSession session, string[] parts) // Cambiar Select -> HandleSelect
        {
            try
            {
                var character = await AppDbContext.LoadCharacterBySlot(byte.Parse(parts[2]));
                session.Player = new Player(session, character);
                var map = WorldManager.GetInstance(character.MapId);
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
