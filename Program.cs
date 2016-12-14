using System;

namespace SubTitleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var subTitleFile = new SubTitleFile(@"C:\Users\zhangmi\Dropbox\Code\vsCode\SubTitle\data\Westworld.S01E01.eng.srt", 
            @"C:\Users\zhangmi\Dropbox\Code\vsCode\SubTitle\data\Westworld.S01E01.chs.ass");
            subTitleFile.Process();

            Console.Beep();
        }
    }
}
