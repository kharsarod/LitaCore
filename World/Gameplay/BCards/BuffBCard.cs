using Database.Item;
using Enum.Main.BCardEnum;
using Enum.Main.BuffEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.BCards.Handler;
using World.Gameplay.Interfaces;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Gameplay.BCards
{
    [BCardHandler(BCardType.Buff)]
    public class BuffBCard : IBCard
    {
        public BCard BCard { get; set; }
        public BuffBCard(BCard bCard)
        {
            BCard = bCard;
        }

        public async Task Apply(Player player, short skillVNum)
        {
            var rnd = new Random();
            var getSkill = WorldManager.GetSkill((short)BCard.SkillVNum);
            Console.WriteLine($"Skill: {skillVNum}");
            if (getSkill is null) return;

            var bCards = await WorldManager.GetBCardsFromSkill((short)BCard.SkillVNum);
            switch (BCard.SubType)
            {
                case (BCardEffect)11:
                    var chanceData = BCard.FirstEffectValue;
                    if (rnd.Next(1, 101) <= chanceData)
                    {
                        foreach(var bCard in bCards.Where(x => x.Type == BCardType.Buff))
                        {
                            var getBadBuff = WorldManager.Getbuff((short)bCard.SecondaryEffectValue);
                            if (getBadBuff.BuffType == BuffType.Good)
                            {
                                await player.Session.Player.AddBuff((short)bCard.SecondaryEffectValue);
                                if (getSkill.TargetType == 1 && getSkill.HitType == 2)
                                {
                                    foreach (var otherPlayer in player.Session.Player.GameEntity.GetNearestPlayers(getSkill.TargetRange))
                                    {
                                        await otherPlayer.AddBuff((short)bCard.SecondaryEffectValue);
                                    }
                                }
                            }

                            if (player.TargetEntity != player.GameEntity)
                            {
                                await player.TargetEntity.AddBuff((short)bCard.SecondaryEffectValue);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
