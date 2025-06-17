using Database.Helper;
using Database.Item;
using Database.World;
using Enum.Main.BuffEnum;
using Enum.Main.PlayerEnum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public static class BuffImporter
    {
        public static async Task Import()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string fileCardDat = Path.Combine(Environment.CurrentDirectory, "Parser", "Dat", "Card.dat");
            string fileCardLang_Uk = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_uk_card.txt");
            string fileCardLang_Es = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_es_card.txt");
            List<BuffData> cards = new List<BuffData>();
            Dictionary<string, string> dictionaryIdLang = new Dictionary<string, string>();
            BuffData card = new BuffData();
            BCard bcard;
            List<BCard> bcards = new List<BCard>();
            string line;
            bool itemAreaBegin = false;

            var dicLang_EN = new Dictionary<string, string>();
            var dicLang_ES = new Dictionary<string, string>();

            if (File.Exists(fileCardLang_Uk))
            {
                foreach (var linel in File.ReadLines(fileCardLang_Uk))
                {
                    var parts = linel.Split('\t');
                    if (parts.Length == 2)
                    {
                        dicLang_EN[parts[0]] = parts[1];
                    }
                }
            }

            if (File.Exists(fileCardLang_Es))
            {
                foreach (var linel in File.ReadLines(fileCardLang_Es, Encoding.GetEncoding(1252)))
                {
                    var parts = linel.Split('\t');
                    if (parts.Length == 2)
                    {
                        dicLang_ES[parts[0]] = parts[1];
                    }
                }
            }

            using (StreamReader npcIdStream = new StreamReader(fileCardDat, Encoding.GetEncoding(1252)))
            {
                while ((line = npcIdStream.ReadLine()) != null)
                {
                    string[] currentLine = line.Split('\t');

                    if (currentLine.Length > 2 && currentLine[1] == "VNUM")
                    {
                        card = new BuffData
                        {
                            BuffId = short.Parse(currentLine[2])
                        };
                        itemAreaBegin = true;
                       // await WorldDbHelper.DeleteBuffByBuffId(card.BuffId);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "NAME")
                    {
                        string key = currentLine[2];
                        if (dicLang_EN.TryGetValue(key, out string nameEN))
                        {
                            card.Translations.Add(new BuffTranslation
                            {
                                Language = Language.ENGLISH,
                                Name = nameEN
                            });
                        }

                        if (dicLang_ES.TryGetValue(key, out string nameES))
                        {
                            card.Translations.Add(new BuffTranslation
                            {
                                Language = Language.SPANISH,
                                Name = nameES
                            });
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "GROUP")
                    {
                        if (!itemAreaBegin)
                        {
                            continue;
                        }
                        card.Level = byte.Parse(currentLine[3]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "EFFECT")
                    {
                        card.EffectId = int.Parse(currentLine[2]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "STYLE")
                    {
                        card.BuffType = (BuffType)byte.Parse(currentLine[3]);
                        if (card.BuffId == 106)
                        {
                            card.BuffType = BuffType.Bad;
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "TIME")
                    {
                        card.DurationMs = int.Parse(currentLine[2]);
                        card.ActivationDelayMs = int.Parse(currentLine[3]);
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "1ST")
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (currentLine[2 + (i * 6)] != "-1" && currentLine[2 + (i * 6)] != "0")
                            {
                                int first = int.Parse(currentLine[6 + (i * 6)]);

                                bcard = new BCard
                                {
                                    BuffId = card.BuffId,
                                    Type = (Enum.Main.BCardEnum.BCardType)byte.Parse(currentLine[2 + (i * 6)]),
                                    SubType = (Enum.Main.BCardEnum.BCardEffect)((byte.Parse(currentLine[3 + (i * 6)]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),

                                    IsLevelScaled = Convert.ToBoolean(first % 4),
                                    IsLevelDivided = Math.Abs(first % 4) == 2,
                                    FirstEffectValue = first / 4,
                                    SecondaryEffectValue = int.Parse(currentLine[7 + (i * 6)]) / 4,
                                    ThirdEffectValue = int.Parse(currentLine[5 + (i * 6)])
                                };
                                bcards.Add(bcard);
                            }
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "2ST")
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int first = int.Parse(currentLine[6 + (i * 6)]);
                            if (currentLine[2 + (i * 6)] != "-1" && currentLine[2 + (i * 6)] != "0")
                            {
                                bcard = new BCard
                                {
                                    CastType = 1,
                                    BuffId = card.BuffId,
                                    Type = (Enum.Main.BCardEnum.BCardType)byte.Parse(currentLine[2 + (i * 6)]),
                                    SubType = (Enum.Main.BCardEnum.BCardEffect)((byte.Parse(currentLine[3 + (i * 6)]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),

                                    ThirdEffectValue = int.Parse(currentLine[5 + (i * 6)]) / 4,
                                    IsLevelScaled = Convert.ToBoolean(first % 4),
                                    IsLevelDivided = Math.Abs(first % 4) == 2,
                                    FirstEffectValue = first / 4,
                                    SecondaryEffectValue = int.Parse(currentLine[7 + (i * 6)]) / 4
                                };
                                bcards.Add(bcard);
                            }
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "LAST")
                    {
                        card.ExpirationBuffId = short.Parse(currentLine[2]);
                        card.ExpirationBuffChance = byte.Parse(currentLine[3]);

                        cards.Add(card);
                        itemAreaBegin = false;
                    }
                }

                BCard returnBCard(short BuffId, byte type, byte subType, int firstData, int secondData = 0, int thirdData = 0, byte castType = 0, bool isLevelScaled = false, bool isLevelDivided = false)
                {
                    return new BCard
                    {
                        BuffId = BuffId,
                        Type = (Enum.Main.BCardEnum.BCardType)type,
                        SubType = (Enum.Main.BCardEnum.BCardEffect)subType,
                        FirstEffectValue = firstData,
                        SecondaryEffectValue = secondData,
                        ThirdEffectValue = thirdData,
                        CastType = castType,
                        IsLevelScaled = isLevelScaled,
                        IsLevelDivided = isLevelDivided
                    };
                }

                bcards.Add(returnBCard(146, 44, 6, 50));
                bcards.Add(returnBCard(131, 8, 2, 30));
                bcards.Add(returnBCard(131, 8, 3, 30));
                bcards.Add(returnBCard(131, 8, 4, 30));
                bcards.Add(returnBCard(131, 8, 5, 30));
                // Verificar si las buffs ya existen en la base de datos, si ya existen no se insertan.

                await WorldDbHelper.InsertBuffsAsync(cards);
                await WorldDbHelper.InsertBuffBCardsAsync(bcards);

                stopwatch.Stop();
                Log.Information("Buff data imported in {Miliseconds}ms, total buff: {Total}", stopwatch.ElapsedMilliseconds, cards.Count);
            }
        }
    }
}
