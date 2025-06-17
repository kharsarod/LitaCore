using Database.Item;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enum.Main.ItemEnum;
using Enum.Main.PlayerEnum;
using System.IO.Pipelines;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Enum.Main.ShellEnum;
using Enum.Main.BCardEnum;
using Database.Context;
using Database.Helper;

namespace GameDataImporter.Importers
{
    public static class ItemImporter
    {
        public static async Task Import()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string itemDataPath = Path.Combine(Environment.CurrentDirectory, "Parser", "Dat", "item.dat");
            string itemLangPath_EN = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_uk_item.txt");
            string itemLangPath_ES = Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_es_item.txt");

            // Diccionarios de traducción
            var dicLang_EN = new Dictionary<string, string>();
            var dicLang_ES = new Dictionary<string, string>();

            // Leer traducciones inglés
            if (File.Exists(itemLangPath_EN))
            {
                foreach (var line in File.ReadLines(itemLangPath_EN))
                {
                    var parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        dicLang_EN[parts[0]] = parts[1];
                    }
                }
            }

            // Leer traducciones español
            if (File.Exists(itemLangPath_ES))
            {
                foreach (var line in File.ReadLines(itemLangPath_ES, Encoding.GetEncoding(1252)))
                {
                    var parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        dicLang_ES[parts[0]] = parts[1];
                    }
                }
            }

            var items = new List<Item>();
            Item currentItem = new();
            bool itemAreaBegin = false;

            using var stream = new FileStream(itemDataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            using var reader = new StreamReader(stream, Encoding.GetEncoding(1252));
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                switch (parts[0])
                {
                    case "VNUM":
                        itemAreaBegin = true;
                        currentItem.Id = short.Parse(parts[1]);
                        currentItem.Price = parts.Length > 2 ? long.Parse(parts[2]) : 0;
                        currentItem.SellToNpcPrice = currentItem.Price / 20;
                        break;

                    case "END":
                        if (!itemAreaBegin) continue;

                        items.Add(currentItem);
                        currentItem = new();
                        itemAreaBegin = false;
                        break;

                    case "NAME":
                        if (currentItem == null) break;
                        string key = parts[1];

                        if (dicLang_EN.TryGetValue(key, out string nameEN))
                        {
                            currentItem.Translations.Add(new ItemTranslation
                            {
                                Language = Language.ENGLISH,
                                Name = nameEN
                            });
                        }

                        if (dicLang_ES.TryGetValue(key, out string nameES))
                        {
                            currentItem.Translations.Add(new ItemTranslation
                            {
                                Language = Language.SPANISH,
                                Name = nameES
                            });
                        }

                        break;

                    case "INDEX":
                        switch (byte.Parse(parts[1]))
                        {
                            case 4:
                                currentItem.Type = InventoryType.EQUIPMENT;
                                break;

                            case 8:
                                currentItem.Type = InventoryType.EQUIPMENT;
                                break;

                            case 9:
                                currentItem.Type = InventoryType.MAIN;
                                break;

                            case 10:
                                currentItem.Type = InventoryType.ETC;
                                break;

                            default:
                                currentItem.Type = (InventoryType)System.Enum.Parse(typeof(InventoryType), parts[1]);
                                break;
                        }
                        currentItem.ItemType = (parts[2] != "-1" ? (ItemType)System.Enum.Parse(typeof(ItemType), $"{(byte)currentItem.Type}{parts[2]}") : ItemType.WEAPON);
                        currentItem.ItemSubType = byte.Parse(parts[3]);
                        currentItem.EquipmentTypeSlot = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), parts[4]);
                        currentItem.Model = short.Parse(parts[6]);
                        break;

                    case "TYPE":
                        currentItem.AttackType = (AttackType)System.Enum.Parse(typeof(AttackType), parts[1]);
                        currentItem.RequiredClass = currentItem.EquipmentTypeSlot == EquipmentType.FAIRY ? (byte)15 : byte.Parse(parts[2]);
                        break;

                    case "FLAG":
                        currentItem.IsSoldable = !ConvertToBool(parts[5 - 1]);
                        currentItem.IsDroppable = !ConvertToBool(parts[6 - 1]);
                        currentItem.IsTradable = !ConvertToBool(parts[7 - 1]);
                        currentItem.IsMinilandBlocked = ConvertToBool(parts[8 - 1]);
                        currentItem.IsWarehouse = ConvertToBool(parts[9 - 1]);
                        currentItem.ShowWarningOnUse = ConvertToBool(parts[10 - 1]);
                        currentItem.IsTimeSpaceRewardBox = ConvertToBool(parts[11 - 1]);
                        currentItem.ShowDescriptionOnHover = ConvertToBool(parts[12 - 1]);
                        currentItem.UnknownFlag13 = ConvertToBool(parts[13 - 1]);
                        currentItem.FollowMouseOnUse = ConvertToBool(parts[14 - 1]);
                        currentItem.ShowSomethingOnHover = ConvertToBool(parts[15 - 1]);
                        currentItem.IsColored = ConvertToBool(parts[16 - 1]);

                        currentItem.Sex = ConvertToBool(parts[18 - 1]) ? (byte)1 :
                                          ConvertToBool(parts[17 - 1]) ? (byte)2 : (byte)0;

                        currentItem.SoundOnPickup = ConvertToBool(parts[20 - 1]);
                        currentItem.UseReputationAsPrice = ConvertToBool(parts[21 - 1]);

                        if (currentItem.UseReputationAsPrice)
                        {
                            currentItem.ReputPrice = currentItem.Price;
                        }

                        currentItem.IsHeroic = ConvertToBool(parts[22 - 1]);
                        currentItem.Flag7 = ConvertToBool(parts[23 - 1]);
                        currentItem.IsLimited = ConvertToBool(parts[24 - 1]);
                        break;

                    case "DATA":
                        switch (currentItem.ItemType)
                        {
                            case ItemType.WEAPON:
                                currentItem.LevelMinimum = Convert.ToByte(parts[1]);
                                currentItem.DamageMinimum = Convert.ToInt16(parts[2]);
                                currentItem.DamageMaximum = Convert.ToInt16(parts[3]);
                                currentItem.HitRate = Convert.ToInt16(parts[4]);
                                currentItem.CriticalLuckRate = byte.Parse(parts[5]);
                                currentItem.CriticalRate = Convert.ToInt16(parts[6]);
                                currentItem.DefaultUpgrade = Convert.ToByte(parts[9]);
                                currentItem.MaximumAmmo = 100;
                                break;

                            case ItemType.ARMOR:
                                currentItem.LevelMinimum = Convert.ToByte(parts[1]);
                                currentItem.CloseDefence = Convert.ToInt16(parts[2]);
                                currentItem.DistanceDefence = Convert.ToInt16(parts[3]);
                                currentItem.MagicDefence = Convert.ToInt16(parts[4]);
                                currentItem.DefenceDodge = Convert.ToInt16(parts[5]);
                                currentItem.DistanceDefenceDodge = Convert.ToInt16(parts[6]);
                                currentItem.DefaultUpgrade = Convert.ToByte(parts[9]);
                                break;

                            case ItemType.FOOD:
                                currentItem.Hp = short.Parse(parts[1]);
                                currentItem.Mp = short.Parse(parts[3]);
                                break;

                            case ItemType.FASHION:
                                currentItem.LevelMinimum = byte.Parse(parts[1]);
                                currentItem.CloseDefence = short.Parse(parts[2]);
                                currentItem.DistanceDefence = short.Parse(parts[3]);
                                currentItem.MagicDefence = short.Parse(parts[4]);
                                currentItem.DefenceDodge = short.Parse(parts[5]);
                                if (currentItem.EquipmentTypeSlot.Equals(EquipmentType.COSTUME_HAT) || currentItem.EquipmentTypeSlot.Equals(EquipmentType.COSTUME_SUIT))
                                {
                                    currentItem.ItemValidTime = int.Parse(parts[12]) * 3600;
                                }
                                break;

                            case ItemType.JEWELERY:
                                if (currentItem.EquipmentTypeSlot.Equals(EquipmentType.AMULET))
                                {
                                    currentItem.LevelMinimum = byte.Parse(parts[1]);
                                    if ((currentItem.Id > 4055 && currentItem.Id < 4061) || (currentItem.Id > 4172 && currentItem.Id < 4176) || (currentItem.Id > 4045 && currentItem.Id < 4056)
                                     || (currentItem.Id > 8104 && currentItem.Id < 8115) || currentItem.Id == 967 || currentItem.Id == 968)
                                    {
                                        currentItem.ItemValidTime = 10800;
                                    }
                                    else
                                    {
                                        currentItem.ItemValidTime = int.Parse(parts[2]) / 10;
                                    }
                                }
                                else if (currentItem.EquipmentTypeSlot.Equals(EquipmentType.FAIRY))
                                {
                                    currentItem.Element = byte.Parse(parts[1]);
                                    currentItem.ElementRate = short.Parse(parts[2]);
                                    if (currentItem.Id <= 256)
                                    {
                                        currentItem.MaxElementRate = 50;
                                    }
                                    else
                                    {
                                        switch (currentItem.ElementRate)
                                        {
                                            case 0:
                                                if (currentItem.Id >= 800 && currentItem.Id <= 804)
                                                {
                                                    currentItem.MaxElementRate = 50;
                                                }
                                                else
                                                {
                                                    currentItem.MaxElementRate = 70;
                                                }
                                                break;

                                            case 30:
                                                if (currentItem.Id >= 884 && currentItem.Id <= 887)
                                                {
                                                    currentItem.MaxElementRate = 50;
                                                }
                                                else
                                                {
                                                    currentItem.MaxElementRate = 30;
                                                }
                                                break;

                                            case 35:
                                                currentItem.MaxElementRate = 35;
                                                break;

                                            case 40:
                                                currentItem.MaxElementRate = 70;
                                                break;

                                            case 50:
                                                currentItem.MaxElementRate = 80;
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    currentItem.LevelMinimum = byte.Parse(parts[1]);
                                    currentItem.MaxCellonLvl = short.Parse(parts[2]);
                                    currentItem.MaxCellon = short.Parse(parts[3]);
                                }
                                break;

                            case ItemType.EVENT:
                                switch (currentItem.Id)
                                {
                                    case 1332:
                                        currentItem.EffectData = 5108;
                                        break;

                                    case 1333:
                                        currentItem.EffectData = 5109;
                                        break;

                                    case 1334:
                                        currentItem.EffectData = 5111;
                                        break;

                                    case 1335:
                                        currentItem.EffectData = 5107;
                                        break;

                                    case 1336:
                                        currentItem.EffectData = 5106;
                                        break;

                                    case 1337:
                                        currentItem.EffectData = 5110;
                                        break;

                                    case 1339:
                                        currentItem.EffectData = 5114;
                                        break;

                                    case 9031:
                                        currentItem.EffectData = 5108;
                                        break;

                                    case 9032:
                                        currentItem.EffectData = 5109;
                                        break;

                                    case 9033:
                                        currentItem.EffectData = 5011;
                                        break;

                                    case 9034:
                                        currentItem.EffectData = 5107;
                                        break;

                                    case 9035:
                                        currentItem.EffectData = 5106;
                                        break;

                                    case 9036:
                                        currentItem.EffectData = 5110;
                                        break;

                                    case 9038:
                                        currentItem.EffectData = 5114;
                                        break;

                                    // EffectItems aka. fireworks
                                    case 1581:
                                        currentItem.EffectData = 860;
                                        break;

                                    case 1582:
                                        currentItem.EffectData = 861;
                                        break;

                                    case 1585:
                                        currentItem.EffectData = 859;
                                        break;

                                    case 1983:
                                        currentItem.EffectData = 875;
                                        break;

                                    case 1984:
                                        currentItem.EffectData = 876;
                                        break;

                                    case 1985:
                                        currentItem.EffectData = 877;
                                        break;

                                    case 1986:
                                        currentItem.EffectData = 878;
                                        break;

                                    case 1987:
                                        currentItem.EffectData = 879;
                                        break;

                                    case 1988:
                                        currentItem.EffectData = 880;
                                        break;

                                    case 9044:
                                        currentItem.EffectData = 859;
                                        break;

                                    case 9059:
                                        currentItem.EffectData = 875;
                                        break;

                                    case 9060:
                                        currentItem.EffectData = 876;
                                        break;

                                    case 9061:
                                        currentItem.EffectData = 877;
                                        break;

                                    case 9062:
                                        currentItem.EffectData = 878;
                                        break;

                                    case 9063:
                                        currentItem.EffectData = 879;
                                        break;

                                    case 9064:
                                        currentItem.EffectData = 880;
                                        break;

                                    default:
                                        currentItem.EffectData = short.Parse(parts[6]);
                                        break;
                                }
                                break;

                            case ItemType.SPECIAL:

                                switch (currentItem.Id)
                                {
                                    case 1246:
                                    case 9020:
                                        currentItem.Effect = 6600;
                                        currentItem.EffectData = 1;
                                        break;

                                    case 1247:
                                    case 9021:
                                        currentItem.Effect = 6600;
                                        currentItem.EffectData = 2;
                                        break;

                                    case 1248:
                                    case 9022:
                                        currentItem.Effect = 6600;
                                        currentItem.EffectData = 3;
                                        break;

                                    case 1249:
                                    case 9023:
                                        currentItem.Effect = 6600;
                                        currentItem.EffectData = 4;
                                        break;

                                    case 5130:
                                    case 9072:
                                        currentItem.Effect = 1006;
                                        break;

                                    case 1272:
                                    case 1858:
                                    case 9047:
                                        currentItem.Effect = 1009;
                                        currentItem.EffectData = 10;
                                        break;

                                    case 1273:
                                    case 9024:
                                        currentItem.Effect = 1009;
                                        currentItem.EffectData = 30;
                                        break;

                                    case 1274:
                                    case 9025:
                                        currentItem.Effect = 1009;
                                        currentItem.EffectData = 60;
                                        break;

                                    case 1279:
                                    case 9029:
                                        currentItem.Effect = 1007;
                                        currentItem.EffectData = 30;
                                        break;

                                    case 1280:
                                    case 9030:
                                        currentItem.Effect = 1007;
                                        currentItem.EffectData = 60;
                                        break;

                                    case 1923:
                                    case 9056:
                                        currentItem.Effect = 1007;
                                        currentItem.EffectData = 10;
                                        break;

                                    case 1275:
                                    case 1886:
                                    case 9026:
                                        currentItem.Effect = 1008;
                                        currentItem.EffectData = 10;
                                        break;

                                    case 1276:
                                    case 9027:
                                        currentItem.Effect = 1008;
                                        currentItem.EffectData = 30;
                                        break;

                                    case 1277:
                                    case 9028:
                                        currentItem.Effect = 1008;
                                        currentItem.EffectData = 60;
                                        break;

                                    case 1949:
                                        currentItem.Effect = 1002;
                                        currentItem.EffectData = 69;
                                        break;

                                    case 5060:
                                    case 9066:
                                        currentItem.Effect = 1003;
                                        currentItem.EffectData = 30;
                                        break;

                                    case 5061:
                                    case 9067:
                                        currentItem.Effect = 1004;
                                        currentItem.EffectData = 7;
                                        break;

                                    case 5062:
                                    case 9068:
                                        currentItem.Effect = 1004;
                                        currentItem.EffectData = 1;
                                        break;

                                    case 5105:
                                        currentItem.Effect = 651;
                                        break;

                                    case 5115:
                                        currentItem.Effect = 652;
                                        break;

                                    case 1981:
                                        currentItem.Effect = 34; // imagined number as for I = √(-1), complex z = a + bi
                                        break;

                                    case 1982:
                                        currentItem.Effect = 6969; // imagined number as for I = √(-1), complex z = a + bi
                                        break;

                                    case 1904:
                                        currentItem.Effect = 1894;
                                        break;

                                    case 1429:
                                        currentItem.Effect = 666;
                                        break;

                                    case 1430:
                                        currentItem.Effect = 666;
                                        currentItem.EffectData = 1;
                                        break;

                                    case 5107:
                                        currentItem.EffectData = 47;
                                        break;

                                    case 5207:
                                        currentItem.EffectData = 50;
                                        break;

                                    case 5519:
                                        currentItem.EffectData = 60;
                                        break;

                                    case 5795:
                                        currentItem.EffectData = 30;
                                        break;

                                    case 5796:
                                        currentItem.EffectData = 60;
                                        break;

                                    default:
                                        if ((currentItem.Id > 5891 && currentItem.Id < 5900) || (currentItem.Id > 9100 && currentItem.Id < 9109))
                                        {
                                            currentItem.Effect = 69; // imagined number as for I = √(-1), complex z = a + bi
                                        }
                                        else if (currentItem.Id > 1893 && currentItem.Id < 1904)
                                        {
                                            currentItem.Effect = 2152;
                                        }
                                        else
                                        {
                                            currentItem.Effect = short.Parse(parts[1]);
                                        }
                                        break;
                                }
                                switch (currentItem.Effect)
                                {
                                    case 150:
                                    case 151:
                                        switch (int.Parse(parts[3]))
                                        {
                                            case 1:
                                                currentItem.EffectData = 30000;
                                                break;

                                            case 2:
                                                currentItem.EffectData = 70000;
                                                break;

                                            case 3:
                                                currentItem.EffectData = 180000;
                                                break;

                                            default:
                                                currentItem.EffectData = int.Parse(parts[3]);
                                                break;
                                        }

                                        break;

                                    case 204:
                                        currentItem.EffectData = 10000;
                                        break;

                                    case 305:
                                        currentItem.EffectData = int.Parse(parts[4]);
                                        currentItem.Model = short.Parse(parts[3]);
                                        break;

                                    default:
                                        currentItem.EffectData = currentItem.EffectData == 0 ? int.Parse(parts[3]) : currentItem.EffectData;
                                        break;
                                }
                                currentItem.WaitDelay = 5000;
                                break;

                            case ItemType.MAGICAL:
                                if (currentItem.Id > 2059 && currentItem.Id < 2070)
                                {
                                    currentItem.Effect = 10;
                                }
                                else
                                {
                                    currentItem.Effect = short.Parse(parts[1]);
                                }
                                currentItem.EffectData = int.Parse(parts[3]);
                                if (byte.TryParse(parts[4], out byte sex) && sex > 0)
                                {
                                    currentItem.Sex = (byte)(sex - 1);
                                }
                                break;

                            case ItemType.SPECIALIST:
                                currentItem.IsPartnerSp = currentItem.ItemSubType == 4;
                                currentItem.Speed = Convert.ToByte(parts[4]);
                                if (currentItem.IsPartnerSp)
                                {
                                    currentItem.Element = Convert.ToByte(parts[3]);
                                    currentItem.ElementRate = Convert.ToInt16(parts[3]);
                                    currentItem.PartnerClass = Convert.ToByte(parts[18]);
                                    currentItem.LevelMinimum = Convert.ToByte(parts[19]);
                                }
                                else
                                {
                                    currentItem.LevelJobMinimum = Convert.ToByte(parts[19]);
                                    currentItem.ReputationMinimum = Convert.ToByte(parts[20]);
                                }

                                currentItem.SpPointsUsage = Convert.ToByte(parts[12]);
                                // currentItem.Model = currentItem.IsPartnerSp ? (byte)(1 + Convert.ToByte(parts[13])) : Convert.ToByte(parts[13]);
                                currentItem.FireResistance = Convert.ToByte(parts[14]);
                                currentItem.WaterResistance = Convert.ToByte(parts[15]);
                                currentItem.LightResistance = Convert.ToByte(parts[16]);
                                currentItem.DarkResistance = Convert.ToByte(parts[17]);

                                var elementdic = new Dictionary<int, int> { { 0, 0 } };
                                if (currentItem.FireResistance != 0)
                                {
                                    elementdic.Add(1, currentItem.FireResistance);
                                }

                                if (currentItem.WaterResistance != 0)
                                {
                                    elementdic.Add(2, currentItem.WaterResistance);
                                }

                                if (currentItem.LightResistance != 0)
                                {
                                    elementdic.Add(3, currentItem.LightResistance);
                                }

                                if (currentItem.DarkResistance != 0)
                                {
                                    elementdic.Add(4, currentItem.DarkResistance);
                                }

                                if (!currentItem.IsPartnerSp)
                                {
                                    currentItem.Element = (byte)elementdic.OrderByDescending(s => s.Value).First().Key;
                                }

                                switch (currentItem.Id)
                                {
                                    case 901:
                                        currentItem.Element = 1;
                                        break;

                                    case 903:
                                        currentItem.Element = 2;
                                        break;

                                    case 906:
                                        currentItem.Element = 3;
                                        break;

                                    case 909:
                                        currentItem.Element = 3;
                                        break;
                                }

                                break;

                            case ItemType.SHELL:
                                byte shellType = Convert.ToByte(parts[4]);

                                currentItem.ShellMinimumLevel = Convert.ToInt16(parts[2]);
                                currentItem.ShellMaximumLevel = Convert.ToInt16(parts[3]);
                                currentItem.ShellType = (ShellType)(currentItem.ItemSubType == 1 ? shellType + 50 : shellType);
                                break;

                            case ItemType.MAIN:
                                currentItem.Effect = Convert.ToInt16(parts[2]);
                                currentItem.EffectData = Convert.ToInt32(parts[4]);
                                break;

                            case ItemType.UPGRADE:
                                currentItem.Effect = Convert.ToInt16(parts[1]);
                                switch (currentItem.Id)
                                {
                                    case (int)ItemsId.EQUIPMENT_NORMAL_SCROLL:
                                        currentItem.EffectData = 26;
                                        break;

                                    case (int)ItemsId.LOWER_SPECIALIST_SCROLL:
                                        currentItem.EffectData = 27;
                                        break;

                                    case (int)ItemsId.HIGHER_SPECIALIST_SCROLL:
                                        currentItem.EffectData = 28;
                                        break;

                                    case (int)ItemsId.SCROLL_CHICKEN:
                                        currentItem.EffectData = 47;
                                        break;

                                    case (int)ItemsId.SCROLL_PYJAMA:
                                        currentItem.EffectData = 50;
                                        break;

                                    case (int)ItemsId.EQUIPMENT_GOLD_SCROLL:
                                        currentItem.EffectData = 61;
                                        break;

                                    case (int)ItemsId.SCROLL_PIRATE:
                                        currentItem.EffectData = 60;
                                        break;

                                    default:
                                        currentItem.EffectData = Convert.ToInt32(parts[3]);
                                        break;
                                }

                                break;

                            case ItemType.PRODUCTION:
                                currentItem.Effect = Convert.ToInt16(parts[1]);
                                currentItem.EffectData = Convert.ToInt32(parts[3]);
                                break;

                            case ItemType.MAP:
                                currentItem.Effect = Convert.ToInt16(parts[1]);
                                currentItem.EffectData = Convert.ToInt32(parts[3]);
                                break;

                            case ItemType.POTION:
                                currentItem.Hp = Convert.ToInt16(parts[1]);
                                currentItem.Mp = Convert.ToInt16(parts[3]);
                                break;

                            case ItemType.SNACK:
                                currentItem.Hp = Convert.ToInt16(parts[1]);
                                currentItem.Mp = Convert.ToInt16(parts[3]);
                                break;

                            case ItemType.PET_PARTNER_ITEM:
                                currentItem.Effect = Convert.ToInt16(parts[1]);
                                currentItem.EffectData = Convert.ToInt32(parts[3]);
                                break;

                            case ItemType.MATERIAL:
                            case ItemType.SELL:
                            case ItemType.QUEST2:
                            case ItemType.QUEST:
                            case ItemType.AMMO:
                                // vacío xd
                                break;
                        }

                        if (currentItem.Type == InventoryType.MINILAND)
                        {
                            currentItem.MinilandObjectPoint = int.Parse(parts[1]);
                            currentItem.EffectData = short.Parse(parts[7]);
                            currentItem.Width = Convert.ToByte(parts[8]) == 0 ? (byte)1 : Convert.ToByte(parts[8]);
                            currentItem.Height = Convert.ToByte(parts[9]) == 0 ? (byte)1 : Convert.ToByte(parts[9]);
                        }

                        if (currentItem.EquipmentTypeSlot == EquipmentType.BOOTS || currentItem.EquipmentTypeSlot == EquipmentType.GLOVES || currentItem.Type == 0)
                        {
                            currentItem.FireResistance = Convert.ToInt16(parts[6]);
                            currentItem.WaterResistance = Convert.ToInt16(parts[7]);
                            currentItem.LightResistance = Convert.ToInt16(parts[8]);
                            currentItem.DarkResistance = Convert.ToInt16(parts[10]);
                        }

                        break;

                    case "BUFF":
                        for (int i = 0; i < 5; i++)
                        {
                            byte type = (byte)(int.Parse(parts[1 + (5 * i)]));
                            if (type != 0 && type != 255)
                            {
                                int first = int.Parse(parts[2 + (5 * i)]);
                                int rawVal = int.Parse(parts[4 + (5 * i)]);
                                BCard itemCard = new BCard
                                {
                                    ItemId = currentItem.Id,
                                    Type = (BCardType)type,
                                    SubType = (BCardEffect)(byte)((int.Parse(parts[4 + (5 * i)]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),
                                    IsLevelScaled = Convert.ToBoolean(first % 4),
                                    IsLevelDivided = Math.Abs(first % 4) == 2,
                                    FirstEffectValue = (short)(first / 4),
                                    SecondaryEffectValue = (short)(int.Parse(parts[3 + (5 * i)]) / 4),
                                    ThirdEffectValue = (short)(int.Parse(parts[5 + (5 * i)]) / 4),
                                };
                                currentItem.BCards.Add(itemCard);
                            }
                        }
                        break;
                }
            }

            // Guardado en base de datos (descomenta si tienes el contexto listo)

            await GenericDbHelper.InsertListAsync<Item, WorldDbContext>(items);

            sw.Stop();
            Log.Information($"Item data imported in {sw.ElapsedMilliseconds}ms.");
        }

        private static bool ConvertToBool(string value)
        {
            return value == "1"; // Devuelve true solo si el valor es '1', de lo contrario, false
        }
    }
}