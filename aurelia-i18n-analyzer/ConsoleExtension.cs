using System;

namespace aureliai18nanalyzer
{
    public static class ConsoleExtension
    {
        public static void WriteErrorLine(string text)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteSuccessLine(string text)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = foregroundColor;
        }
    }
}
