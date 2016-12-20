using System;

namespace SubTitleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var subTitleFile = new SubTitleFile(@"C:\Users\zhangmi\Dropbox\Code\vsCode\SubTitle\data", "Westworld.S01E01.eng.srt", "Westworld.S01E01.chs.ass", @"C:\Users\zhangmi\Dropbox\Code\vsCode\SubTitle\data\result");
            subTitleFile.Process();

            Console.Beep();
        }
    }
}
