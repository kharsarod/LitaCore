using Database.Migrations.WorldDb;
using Enum.Main.BCardEnum;
using Enum.Main.BuffEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Gameplay
{
    public static class BCardSkill
    {
        public static async Task ApplyBCard(ClientSession session, short skillVNum, byte targetType = 0, byte hitType = 0, GameEntity targetEntity = null)
        {
            var rnd = new Random();
            var skillBCards = await WorldManager.GetBCardsFromSkill(skillVNum);
            var bEffs = BCardEffectDefinitions.BCardMapping.BCardEffects;
            var getSkill = WorldManager.GetSkill(skillVNum);
            foreach (var bCard in skillBCards)
            {
                if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes))
                {
                    var includedBCard = BCardType.Buff;

                    if (bCard.Type != includedBCard)
                    {
                        continue;
                    }

                    switch (bCard.SubType)
                    {
                        case (BCardEffect)11:
                            var chanceData = bCard.FirstEffectValue;
                            // Crear una condicional para aplicar el efecto según el porcentaje de probabilidad.
                            if (rnd.Next(1, 101) <= chanceData)
                            {
                                var getBadBuff = WorldManager.Getbuff((short)bCard.SecondaryEffectValue);
                                if (getBadBuff.BuffType == BuffType.Good)
                                {
                                    await session.Player.AddBuff((short)bCard.SecondaryEffectValue);
                                    if (targetType == 1 && hitType == 2)
                                    {
                                        foreach (var otherPlayer in session.Player.GameEntity.GetNearestPlayers(getSkill.TargetRange))
                                        {
                                            await otherPlayer.AddBuff((short)bCard.SecondaryEffectValue);
                                        }
                                    }
                                }

                                if (getBadBuff is null) continue;
                                if (getBadBuff.BuffType == BuffType.Bad)
                                {
                                    await targetEntity.AddBuff((short)bCard.SecondaryEffectValue);
                                }
                            }
                            break;
                    }
                }
            }

            await session.SendPacket(session.Player.Packets.GenerateStat());
        }
    }
}
