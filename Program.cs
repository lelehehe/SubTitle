using System;

namespace SubTitleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var subTitleFile = new SubTitleFile(@".\data", "Westworld.S01E01.eng.srt", "Westworld.S01E01.chs.ass", @".\data\result");
            subTitleFile.Process();

            Console.Beep();
        }
    }
}
