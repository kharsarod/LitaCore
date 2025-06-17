using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enum.Main.PlayerEnum;

namespace World.Extensions
{
    public static class PlayerEncoding
    {
        public static Encoding GetPlayerEncoding(Language lang)
        {
            switch (lang)
            {
                case Language.ENGLISH:
                case Language.SPANISH:
                case Language.FRENCH:
                    return Encoding.GetEncoding(1252);

                case Language.DEUTSCH:
                case Language.POLISH:
                case Language.ITALIAN:
                case Language.CZECH:
                    return Encoding.GetEncoding(1250);

                case Language.TURKISH:
                    return Encoding.GetEncoding(1254);

                default:
                    return Encoding.GetEncoding(1252);
            }
        }
    }
}