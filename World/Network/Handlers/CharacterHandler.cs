using Database.Player;
using Enum.Main.CharacterEnum;
using NosCryptLib.Encryption;
using NosTalePacketsLib.Packets.Client;
using NosTalePacketsLib.Packets.Parser;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Dignity = 0, // cargar cuenta con el loadByName, ya que al entrar sale el NOS y sale el nombre de la cuenta, sino es el NOS es el NsTeST.
                Slot = byte.Parse(parts[3]),
                Gender = (Gender)byte.Parse(parts[4]),
                HairStyle = (HairStyle)byte.Parse(parts[5]),
                HairColor = (HairColor)byte.Parse(parts[6]),
                Class = ClassId.Adventurer,
                Gold = 1500
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
                await session.SendPacket("OK");
               // await session.SendPacket("lbs 0");
                await session.SendPacket("glist 0 0");
                await session.SendPacket("in 1 [GS]Player123 - 532 123 876 2 1 3 45 2 654.321.543.678.987.345.234.123 76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23");
                await session.SendPacket("mapout");
                await session.SendPacket("rsfi 1 1 0 9 0 9");
                await session.SendPacket("c_info Kharsarod - -1 -1 - 1 0 0 1 1 1 27 0 0 0 0 0 0");
                await session.SendPacket("c_close 1");
                await session.SendPacket("c_mode 1 1 0 0 0 0 10 0");
                await session.SendPacket("at 1 1 99 99 0 0 12 2 -1");
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
            }
        }
    }
}
