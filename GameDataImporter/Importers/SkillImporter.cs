using Database.Helper;
using Database.Item;
using Database.World;
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
    public static class SkillImporter
    {
        public static async Task Import()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string fileSkillId = Path.Combine(Environment.CurrentDirectory, "Parser", "Dat", "Skill.dat");
            string fileSkillLang = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_uk_skill.txt");
            List<SkillData> skills = new List<SkillData>();

            Dictionary<string, string> dictionaryIdLang = new Dictionary<string, string>();
            SkillData skill = new SkillData();
            List<ComboData> Combos = new();
            List<BCard> skillCards = new();
            string line;
            using (StreamReader skillIdLangStream = new StreamReader(fileSkillLang, Encoding.GetEncoding(1252)))
            {
                while ((line = skillIdLangStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split('\t');
                    if (linesave.Length > 1 && !dictionaryIdLang.ContainsKey(linesave[0]))
                    {
                        dictionaryIdLang.Add(linesave[0], linesave[1]);
                    }
                }
            }

            using (StreamReader skillIdStream = new StreamReader(fileSkillId, Encoding.GetEncoding(1252)))
            {
                while ((line = skillIdStream.ReadLine()) != null)
                {
                    string[] currentLine = line.Split('\t');

                    if (currentLine.Length > 2 && currentLine[1] == "VNUM")
                    {
                        skill = new SkillData
                        {
                            SkillVNum = short.Parse(currentLine[2])
                        };
                        //  DAOFactory.BCardDAO.DeleteBySkillVNum(skill.SkillVNum);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "NAME")
                    {
                        skill.Name = dictionaryIdLang.TryGetValue(currentLine[2], out string name) ? name : "";
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "TYPE")
                    {
                        skill.SkillType = byte.Parse(currentLine[2]);
                        skill.CastId = short.Parse(currentLine[3]);
                        skill.Class = byte.Parse(currentLine[4]);
                        skill.Type = byte.Parse(currentLine[5]);
                        skill.Element = byte.Parse(currentLine[7]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "FCOMBO")
                    {
                        for (int i = 3; i < currentLine.Length - 4; i += 3)
                        {
                            ComboData combo = new ComboData
                            {
                                SkillVNum = skill.SkillVNum,
                                Hit = short.Parse(currentLine[i]),
                                Animation = short.Parse(currentLine[i + 1]),
                                Effect = short.Parse(currentLine[i + 2])
                            };

                            if (combo.Hit != 0 || combo.Animation != 0 || combo.Effect != 0)
                            {
                                var combos = await WorldDbHelper.LoadComboByVNumHitAndEffect(combo.SkillVNum, combo.Hit, combo.Effect);
                                if (!combos.Any())
                                {
                                    Combos.Add(combo);
                                }
                            }
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "COST")
                    {
                        skill.CPCost = currentLine[2] == "-1" ? (byte)0 : byte.Parse(currentLine[2]);
                        skill.Price = int.Parse(currentLine[3]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "LEVEL")
                    {
                        skill.LevelMinimum = currentLine[2] != "-1" ? byte.Parse(currentLine[2]) : (byte)0;
                        if (skill.Class > 31)
                        {
                            SkillData firstskill = skills.Find(s => s.Class == skill.Class);
                            if (firstskill == null || skill.SkillVNum <= firstskill.SkillVNum + 10)
                            {
                                switch (skill.Class)
                                {
                                    case 8:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 3:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 2:
                                                skill.LevelMinimum = 10;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }
                                        break;

                                    case 9:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 9:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 8:
                                                skill.LevelMinimum = 16;
                                                break;

                                            case 7:
                                                skill.LevelMinimum = 12;
                                                break;

                                            case 6:
                                                skill.LevelMinimum = 8;
                                                break;

                                            case 5:
                                                skill.LevelMinimum = 4;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }
                                        break;

                                    case 16:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 6:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 5:
                                                skill.LevelMinimum = 15;
                                                break;

                                            case 4:
                                                skill.LevelMinimum = 10;
                                                break;

                                            case 3:
                                                skill.LevelMinimum = 5;
                                                break;

                                            case 2:
                                                skill.LevelMinimum = 3;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }
                                        break;

                                    default:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 10:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 9:
                                                skill.LevelMinimum = 16;
                                                break;

                                            case 8:
                                                skill.LevelMinimum = 12;
                                                break;

                                            case 7:
                                                skill.LevelMinimum = 8;
                                                break;

                                            case 6:
                                                skill.LevelMinimum = 4;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        skill.MinimumAdventurerLevel = currentLine[3] != "-1" ? byte.Parse(currentLine[3]) : (byte)0;
                        skill.MinimumSwordmanLevel = currentLine[4] != "-1" ? byte.Parse(currentLine[4]) : (byte)0;
                        skill.MinimumArcherLevel = currentLine[5] != "-1" ? byte.Parse(currentLine[5]) : (byte)0;
                        skill.MinimumMagicianLevel = currentLine[6] != "-1" ? byte.Parse(currentLine[6]) : (byte)0;
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "EFFECT")
                    {
                        skill.CastEffect = short.Parse(currentLine[3]);
                        skill.CastAnimation = short.Parse(currentLine[4]);
                        skill.Effect = short.Parse(currentLine[5]);
                        skill.AttackAnimation = short.Parse(currentLine[6]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "TARGET")
                    {
                        skill.TargetType = byte.Parse(currentLine[2]);
                        skill.HitType = byte.Parse(currentLine[3]);
                        skill.Range = byte.Parse(currentLine[4]);
                        skill.TargetRange = (byte)short.Parse(currentLine[5]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "DATA")
                    {
                        skill.UpgradeSkill = short.Parse(currentLine[2]);
                        skill.UpgradeType = short.Parse(currentLine[3]);
                        skill.CastTime = short.Parse(currentLine[6]);
                        skill.Cooldown = short.Parse(currentLine[7]);
                        skill.MpCost = short.Parse(currentLine[10]);
                        skill.ItemVNum = short.Parse(currentLine[12]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "BASIC")
                    {
                        byte type = (byte)(int.Parse(currentLine[3]));
                        if (type != 0 && type != 255)
                        {
                            int first = int.Parse(currentLine[5]);
                            if (type == 75 && first == -100)
                            {
                                first = 100;
                            }
                            BCard itemCard = new BCard
                            {
                                SkillVNum = skill.SkillVNum,
                                Type = (Enum.Main.BCardEnum.BCardType)type,
                                SubType = (Enum.Main.BCardEnum.BCardEffect)((byte)(int.Parse(currentLine[4]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),
                                IsLevelScaled = Convert.ToBoolean(first % 4),
                                IsLevelDivided = Math.Abs(first % 4) == 2,
                                FirstEffectValue = (short)(first / 4),
                                SecondaryEffectValue = (short)(int.Parse(currentLine[6]) / 4),
                                ThirdEffectValue = (short)(int.Parse(currentLine[7]) / 4),
                            };
                            skillCards.Add(itemCard);
                        }
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "FCOMBO")
                    {
                        // investigate
                        /*
                        if (currentLine[2] == "1")
                        {
                            combo.FirstActivationHit = byte.Parse(currentLine[3]);
                            combo.FirstComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FirstComboEffect = short.Parse(currentLine[5]);
                            combo.SecondActivationHit = byte.Parse(currentLine[3]);
                            combo.SecondComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.SecondComboEffect = short.Parse(currentLine[5]);
                            combo.ThirdActivationHit = byte.Parse(currentLine[3]);
                            combo.ThirdComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.ThirdComboEffect = short.Parse(currentLine[5]);
                            combo.FourthActivationHit = byte.Parse(currentLine[3]);
                            combo.FourthComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FourthComboEffect = short.Parse(currentLine[5]);
                            combo.FifthActivationHit = byte.Parse(currentLine[3]);
                            combo.FifthComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FifthComboEffect = short.Parse(currentLine[5]);
                        }
                        */
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "CELL")
                    {
                        // investigate
                    }
                    else if (currentLine.Length > 1 && currentLine[1] == "Z_DESC")
                    {
                        // investigate
                        skills.Add(skill);
                    }
                }
                // skills = skills.Where(s => WorldDbHelper.LoadSkillByVNumAsync(s.SkillVNum) == null).ToList();

                await WorldDbHelper.InsertSkillsAsync(skills);
                await WorldDbHelper.InsertSkillsBcards(skillCards);
                await WorldDbHelper.InsertCombosAsync(Combos);
                sw.Stop();

                // Log.Information
                Log.Information("Imported {Count} skills in {ElapsedMilliseconds}ms.", skills.Count, sw.ElapsedMilliseconds);
            }
        }
    }
}