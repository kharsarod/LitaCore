using Database.Helper;
using Database.Item;
using Database.MapEntity;
using Database.Migrations.WorldDb;
using Database.MonsterData;
using Database.World;
using Enum.Main.MonsterEnum;
using Enum.Main.PlayerEnum;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataImporter.Importers
{
    public static class NpcMonsterImporter
    {
        public static async Task Import()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int[] basicHp = new int[101];
            int[] basicMp = new int[101];
            int[] basicXp = new int[101];
            int[] basicJXp = new int[101];

            // basicHpLoad
            int baseHp = 138;
            int basup = 17;
            for (int i = 0; i < 100; i++)
            {
                basicHp[i] = baseHp;
                basup++;
                baseHp += basup;

                if (i == 37)
                {
                    baseHp = 1765;
                    basup = 65;
                }
                if (i >= 41 && (99 - i) % 8 == 0)
                {
                    basup++;
                }
            }

            const int baseMp = 10;
            int baseMpup = 5;
            // basicMpLoad
            int x = 0;
            for (int i = 0; i < 100; i++)
            {
                basicMp[i] = i == 0 ? baseMp : basicMp[i - 1];

                if (i == 1)
                {
                    continue;
                }
                if (i > 3)
                {
                    if (x != 3)
                    {
                        x++;
                    }
                    else
                    {
                        x = 0;
                    }
                }

                if (x > 1)
                {
                    baseMpup++;
                }

                basicMp[i] += baseMpup;
            }

            // basicXPLoad
            for (int i = 0; i < 100; i++)
            {
                basicXp[i] = i * 180;
            }

            // basicJXpLoad
            for (int i = 0; i < 100; i++)
            {
                basicJXp[i] = 360;
            }

            string fileNpcId = Path.Combine(Environment.CurrentDirectory, "Parser", "Dat", "monster.dat");
            string fileNpcLang = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_uk_monster.txt");
            List<NpcMonster> npcs = new();

            Dictionary<string, string> dictionaryIdLang = new Dictionary<string, string>();
            NpcMonster npc = new();
            List<DropData> drops = new();
            List<BCard> monstercards = new();
            List<NpcMonsterSkill> skills = new();
            string line;
            bool itemAreaBegin = false;
            long unknownData = 0;
            using (StreamReader npcIdLangStream = new StreamReader(fileNpcLang, Encoding.GetEncoding(1252)))
            {
                while ((line = npcIdLangStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split('\t');
                    if (linesave.Length > 1 && !dictionaryIdLang.ContainsKey(linesave[0]))
                    {
                        dictionaryIdLang.Add(linesave[0], linesave[1]);
                    }
                }
            }
            using (StreamReader npcIdStream = new StreamReader(fileNpcId, Encoding.GetEncoding(1252)))
            {
                while ((line = npcIdStream.ReadLine()) != null)
                {
                    string[] currentLine = line.Split('\t');

                    if (currentLine.Length > 2 && currentLine[1] == "VNUM")
                    {
                        npc = new NpcMonster
                        {
                            VNum = short.Parse(currentLine[2])
                        };
                        itemAreaBegin = true;
                        unknownData = 0;
                        // await WorldDbHelper.DeleteNpcMonsterByVNum(npc.VNum);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "NAME")
                    {
                        npc.Name = dictionaryIdLang.ContainsKey(currentLine[2]) ? dictionaryIdLang[currentLine[2]] : "";
                    }
                    // Add level max is 100 for monster now. Change it to 99 in txt file or modify the code
                    else if (currentLine.Length > 2 && currentLine[1] == "LEVEL")
                    {
                        if (!itemAreaBegin)
                        {
                            continue;
                        }
                        npc.Level = byte.Parse(currentLine[2]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "RACE")
                    {
                        npc.Race = byte.Parse(currentLine[2]);
                        npc.RaceType = byte.Parse(currentLine[3]);
                    }
                    else if (currentLine.Length > 7 && currentLine[1] == "ATTRIB")
                    {
                        npc.Element = byte.Parse(currentLine[2]);
                        npc.ElementRate = short.Parse(currentLine[3]);
                        npc.FireResistance = short.Parse(currentLine[4]);
                        npc.WaterResistance = short.Parse(currentLine[5]);
                        npc.LightResistance = short.Parse(currentLine[6]);
                        npc.DarkResistance = short.Parse(currentLine[7]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "HP/MP")
                    {
                        npc.MaxHP = int.Parse(currentLine[2]) + basicHp[npc.Level];
                        npc.MaxMP = int.Parse(currentLine[3]) + basicMp[npc.Level];
                        switch (npc.Race)
                        {
                            // TODO: Race Types 1, 2, 4, 5 and 7 are either missing or not 100% correct - test it.
                            case 2:
                            case 3:
                                npc.MaxMP += (npc.Level * 4) + 46;
                                break;

                            case 6:
                                npc.MaxMP += 715;
                                break;

                            case 8:
                                npc.MaxMP = 4;
                                break;
                        }
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "EXP")
                    {
                        npc.XP = Math.Abs(int.Parse(currentLine[2]) + basicXp[npc.Level]);
                        npc.JobXP = int.Parse(currentLine[3]) + basicJXp[npc.Level];
                        switch (npc.VNum)
                        {
                            // Act6 monsters hero exp
                            case 2500:
                                npc.HeroXp = 972;
                                break;

                            case 2501:
                                npc.HeroXp = 974;
                                break;

                            case 2502:
                                npc.HeroXp = 997;
                                break;

                            case 2503:
                                npc.HeroXp = 1118;
                                break;

                            case 2505:
                                npc.HeroXp = 871;
                                break;

                            case 2506:
                                npc.HeroXp = 765;
                                break;

                            case 2507:
                                npc.HeroXp = 803;
                                break;

                            case 2508:
                                npc.HeroXp = 825;
                                break;

                            case 2509:
                                npc.HeroXp = 789;
                                break;

                            case 2510:
                            case 2511:
                                npc.HeroXp = 974;
                                break;

                            case 2512:
                                npc.HeroXp = 977;
                                break;

                            case 2513:
                                npc.HeroXp = 1185;
                                break;

                            case 2515:
                                npc.HeroXp = 3854;
                                break;

                            case 2516:
                                npc.HeroXp = 836;
                                break;

                            case 2517:
                                npc.HeroXp = 450;
                                break;

                            case 2518:
                                npc.HeroXp = 911;
                                break;

                            case 2519:
                                npc.HeroXp = 845;
                                break;

                            case 2520:
                                npc.HeroXp = 3857;
                                break;

                            case 2521:
                                npc.HeroXp = 448;
                                break;

                            case 2522:
                                npc.HeroXp = 526;
                                break;

                            case 2523:
                                npc.HeroXp = 603;
                                break;

                            case 2524:
                                npc.HeroXp = 12860;
                                break;

                            case 2525:
                                npc.HeroXp = 462;
                                break;

                            case 2526:
                                npc.HeroXp = 11157;
                                break;

                            case 2527:
                                npc.HeroXp = 18252;
                                break;

                            case 2530:
                                npc.HeroXp = 29060;
                                break;

                            case 2559:
                                npc.HeroXp = 1308;
                                break;

                            case 2560:
                                npc.HeroXp = 1234;
                                break;

                            case 2561:
                                npc.HeroXp = 1168;
                                break;

                            case 2562:
                                npc.HeroXp = 959;
                                break;

                            case 2563:
                                npc.HeroXp = 947;
                                break;

                            case 2564:
                                npc.HeroXp = 974;
                                break;

                            case 2566:
                                npc.HeroXp = 1121;
                                break;

                            case 2567:
                                npc.HeroXp = 1118;
                                break;

                            case 2568:
                                npc.HeroXp = 4397;
                                break;

                            case 2569:
                                npc.HeroXp = 4394;
                                break;

                            case 2570:
                                npc.HeroXp = 4400;
                                break;

                            case 2571:
                                npc.HeroXp = 2205;
                                break;

                            case 2572:
                                npc.HeroXp = 5632;
                                break;

                            case 2573:
                                npc.HeroXp = 3756;
                                break;

                            // act7 monsters hero exp

                            case 3000:
                                npc.HeroXp = 612;
                                break;

                            case 3001:
                                npc.HeroXp = 686;
                                break;

                            case 3002:
                                npc.HeroXp = 1258;
                                break;

                            case 3003:
                                npc.HeroXp = 452;
                                break;

                            case 3004:
                                npc.HeroXp = 1229;
                                break;

                            case 3005:
                                npc.HeroXp = 1159;
                                break;

                            case 3006:
                                npc.HeroXp = 1207;
                                break;

                            case 3007:
                                npc.HeroXp = 1302;
                                break;

                            case 3008:
                                npc.HeroXp = 620;
                                break;

                            case 3009:
                                npc.HeroXp = 1187;
                                break;

                            case 3010:
                                npc.HeroXp = 1258;
                                break;

                            case 3011:
                                npc.HeroXp = 1327;
                                break;

                            case 3012:
                                npc.HeroXp = 1207;
                                break;

                            case 3013:
                                npc.HeroXp = 472;
                                break;

                            case 3014:
                                npc.HeroXp = 497;
                                break;

                            case 3015:
                                npc.HeroXp = 1302;
                                break;

                            case 3016:
                                npc.HeroXp = 1324;
                                break;

                            case 3017:
                            case 3018:
                                npc.HeroXp = 1184;
                                break;

                            case 3019:
                                npc.HeroXp = 617;
                                break;

                            case 3021:
                            case 3022:
                                npc.HeroXp = 1258;
                                break;

                            case 3023:
                            case 3024:
                                npc.HeroXp = 1260;
                                break;

                            case 3025:
                                npc.HeroXp = 1391;
                                break;

                            case 3026:
                                npc.HeroXp = 1394;
                                break;

                            case 3037:
                                npc.HeroXp = 277;
                                break;

                            case 3103:
                                npc.HeroXp = 1496;
                                break;

                            case 3104:
                                npc.HeroXp = 1348;
                                break;

                            case 3105:
                                npc.HeroXp = 2254;
                                break;

                            default:
                                npc.HeroXp = 0;
                                break;
                        }
                    }
                    else if (currentLine.Length > 6 && currentLine[1] == "PREATT")
                    {
                        npc.IsHostile = currentLine[2] != "0";
                        npc.NoticeRange = byte.Parse(currentLine[4]);
                        npc.Speed = byte.Parse(currentLine[5]);
                        npc.RespawnTime = int.Parse(currentLine[6]);
                    }
                    else if (currentLine.Length > 6 && currentLine[1] == "WEAPON")
                    {
                        if (currentLine[3] == "1")
                        {
                            short line2 = (short)(short.Parse(currentLine[2]) - 1);
                            npc.DamageMinimum = (short)((line2 * 4) + 32 + short.Parse(currentLine[4]) + Math.Round(Convert.ToDecimal((npc.Level - 1) / 5)));
                            npc.DamageMaximum = (short)((line2 * 6) + 40 + short.Parse(currentLine[5]) - Math.Round(Convert.ToDecimal((npc.Level - 1) / 5)));
                            npc.Concentrate = (short)((line2 * 5) + 27 + short.Parse(currentLine[6]));
                            npc.CriticalChance = (byte)(4 + short.Parse(currentLine[7]));
                            npc.CriticalRate = (short)(70 + short.Parse(currentLine[8]));
                        }
                        else if (currentLine[3] == "2")
                        {
                            short line2 = short.Parse(currentLine[2]);
                            npc.DamageMinimum = (short)((line2 * 6.5f) + 23 + short.Parse(currentLine[4]));
                            npc.DamageMaximum = (short)(((line2 - 1) * 8) + 38 + short.Parse(currentLine[5]));
                            npc.Concentrate = (short)(70 + short.Parse(currentLine[6]));
                        }
                    }
                    else if (currentLine.Length > 6 && currentLine[1] == "ARMOR")
                    {
                        short line2 = (short)(short.Parse(currentLine[2]) - 1);
                        npc.CloseDefence = (short)((line2 * 2) + 18);
                        npc.DistanceDefence = (short)((line2 * 3) + 17);
                        npc.MagicDefence = (short)((line2 * 2) + 13);
                        npc.DefenceDodge = (short)((line2 * 5) + 31);
                        npc.DistanceDefenceDodge = (short)((line2 * 5) + 31);
                    }
                    else if (currentLine.Length > 7 && currentLine[1] == "ETC")
                    {
                        unknownData = Convert.ToInt64(currentLine[2]);
                        npc.Catch = currentLine[2] == "8";
                        if (currentLine[2] == "72" || currentLine[2] == "520")
                        {
                            npc.Catch = true;
                        }
                        if (unknownData == -2147481593)
                        {
                            npc.MonsterType = MonsterType.Special;
                        }
                        if (unknownData == -2147483616 || unknownData == -2147483647 || unknownData == -2147483646)
                        {
                            npc.NoAggresiveIcon = npc.Race == 8 && npc.RaceType == 0;
                        }
                        if (npc.VNum >= 588 && npc.VNum <= 607)
                        {
                            npc.MonsterType = MonsterType.Elite;
                        }
                    }
                    else if (currentLine.Length > 6 && currentLine[1] == "SETTING")
                    {
                        if (currentLine[4] != "0")
                        {
                            npc.VNumRequired = short.Parse(currentLine[4]);
                            npc.ItemAmountRequired = 1;
                        }
                    }
                    else if (currentLine.Length > 4 && currentLine[1] == "PETINFO")
                    {
                        if (npc.VNumRequired == 0 && (unknownData == -2147481593 || unknownData == -2147481599 || unknownData == -1610610681))
                        {
                            npc.VNumRequired = short.Parse(currentLine[2]);
                            npc.ItemAmountRequired = byte.Parse(currentLine[3]);
                        }
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "EFF")
                    {
                        npc.BasicSkill = short.Parse(currentLine[2]);
                    }
                    else if (currentLine.Length > 8 && currentLine[1] == "ZSKILL")
                    {
                        npc.AttackClass = byte.Parse(currentLine[2]);
                        switch (npc.VNum)
                        {
                            case 45:
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                            case 50:
                            case 51:
                            case 52:
                            case 53: // Pii Pods ^
                            case 195: // Training Stake
                            case 208:
                            case 209: // Beehives ^
                                npc.BasicRange = 0;
                                break;

                            default:
                                npc.BasicRange = byte.Parse(currentLine[3]);
                                break;
                        }
                        npc.BasicArea = byte.Parse(currentLine[5]);
                        npc.BasicCooldown = short.Parse(currentLine[6]);
                    }
                    else if (currentLine.Length > 4 && currentLine[1] == "WINFO")
                    {
                        npc.AttackUpgrade = byte.Parse(unknownData == 1 ? currentLine[2] : currentLine[4]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "AINFO")
                    {
                        npc.DefenceUpgrade = byte.Parse(unknownData == 1 ? currentLine[2] : currentLine[3]);
                    }
                    else if (currentLine.Length > 1 && currentLine[1] == "SKILL")
                    {
                        for (int i = 2; i < currentLine.Length - 3; i += 3)
                        {
                            short vnum = short.Parse(currentLine[i]);
                            if (vnum == -1 || vnum == 0)
                            {
                                continue;
                            }

                            skills.Add(new NpcMonsterSkill
                            {
                                SkillVNum = vnum,
                                Rate = short.Parse(currentLine[i + 1]),
                                VNum = npc.VNum
                            });
                        }
                    }
                    else if (currentLine.Length > 1 && currentLine[1] == "CARD")
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            byte type = (byte)(int.Parse(currentLine[2 + (5 * i)]));
                            if (type != 0 && type != 255)
                            {
                                int first = int.Parse(currentLine[3 + (5 * i)]);
                                BCard itemCard = new BCard
                                {
                                    NpcMonsterVNum = npc.VNum,
                                    Type = (Enum.Main.BCardEnum.BCardType)type,
                                    SubType = (Enum.Main.BCardEnum.BCardEffect)((byte)(int.Parse(currentLine[5 + (5 * i)]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),
                                    IsLevelScaled = Convert.ToBoolean(first % 4),
                                    IsLevelDivided = Math.Abs(first % 4) == 2,
                                    FirstEffectValue = (short)(first / 4),
                                    SecondaryEffectValue = (short)(int.Parse(currentLine[4 + (5 * i)]) / 4),
                                    ThirdEffectValue = (short)(int.Parse(currentLine[6 + (5 * i)]) / 4),
                                };
                                monstercards.Add(itemCard);
                            }
                        }
                    }
                    else if (currentLine.Length > 1 && currentLine[1] == "BASIC")
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            byte type = (byte)(int.Parse(currentLine[2 + (5 * i)]));
                            if (type != 0)
                            {
                                byte subType = (byte)int.Parse(currentLine[6 + (5 * i)]);
                                int firstData = (short)(int.Parse(currentLine[5 + (5 * i)]));
                                int thirdData = (short)(int.Parse(currentLine[3 + (5 * i)]) / 4);
                                BCard itemCard = new BCard
                                {
                                    NpcMonsterVNum = npc.VNum,
                                    Type = (Enum.Main.BCardEnum.BCardType)type,
                                    SubType = (Enum.Main.BCardEnum.BCardEffect)(subType > 0 ? subType : (byte)(firstData + 1)),
                                    FirstEffectValue = subType > 0 ? firstData : thirdData,
                                    SecondaryEffectValue = (short)(int.Parse(currentLine[4 + (5 * i)]) / 4),
                                    ThirdEffectValue = subType > 0 ? thirdData : subType,
                                    CastType = 1,
                                    IsLevelScaled = false,
                                    IsLevelDivided = false
                                };
                                monstercards.Add(itemCard);
                            }
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "ITEM")
                    {
                        var _monster = await WorldDbHelper.LoadNpcMonsterByVNumAsync(npc.VNum);
                        if (_monster is null)
                        {
                            npcs.Add(npc);
                        }

                        for (int i = 2; i < currentLine.Length - 3; i += 3)
                        {
                            short vnum = short.Parse(currentLine[i]);
                            if (vnum == -1)
                            {
                                break;
                            }

                            var _drop = await WorldDbHelper.LoadDropByMonsterAndItemVNumAsync(npc.VNum, vnum);

                            if (_drop is not null)
                            {
                                continue;
                            }

                            drops.Add(new DropData
                            {
                                VNum = vnum,
                                Amount = int.Parse(currentLine[i + 2]),
                                MonsterVNum = npc.VNum,
                                Chance = int.Parse(currentLine[i + 1])
                            });
                        }
                        itemAreaBegin = false;
                    }
                }

                await WorldDbHelper.InsertNpcMonstersAsync(npcs);
                await WorldDbHelper.InsertNpcMonsterSkillsAsync(skills);
                await WorldDbHelper.InsertNpcMonsterCardsAsync(monstercards);
                await WorldDbHelper.InsertNpcMonsterDropsAsync(drops);
            }

            sw.Stop();

            Log.Information($"Monsters loaded in {sw.ElapsedMilliseconds}ms.");
        }
    }
}