using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Utils
{
    public static class ConsoleUtils
    {
        public static void CenterText(string text)
        {
            int width = Console.WindowWidth;
            int padding = (width - text.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(padding, 0)) + text);
        }
    }
}