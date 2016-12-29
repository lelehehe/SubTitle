using System.Collections.Generic;
using System;
using System.IO;

namespace SubTitleApp
{
    public class SubTitleFile
    {
        public string englishSrc { get; set; }
        public string chineseSrc { get; set; }
        public string sourcePath {get; set;}
        public string resultFile {get; set;}

        public SubTitleFile(string sourcePath, string engFile, string chsFile, string resultPath)
        {
            this.sourcePath = sourcePath;
            englishSrc = sourcePath + "\\" + engFile;
            chineseSrc = sourcePath + "\\" + chsFile;
            resultFile = resultPath + "\\" + chsFile;
        }
        public void Process() {
            var chsLines = System.IO.File.ReadAllLines(this.chineseSrc);

            var i = 0; 
            var enumerator = this.getEnglishBlock().GetEnumerator();
            enumerator.MoveNext();
            var engBlock = enumerator.Current;
            string start = null, end = null, mid = null;
            bool transformStarted = false;
            float span = 0;
            while (i < chsLines.Length) {
                while (i < chsLines.Length && (chsLines[i].Length == 0 || !chsLines[i].StartsWith("Dialogue:"))) {
                    i++;
                } 
                var chsUpper = chsLines[i].ToUpper(); 
                var engUpper = engBlock[2].ToUpper();
                if ((i < chsLines.Length - 1) && twoChsInOneBlock(chsLines[i], chsLines[i+1], engBlock)) {
                    //62
                    // 00:05:49,912 --> 00:05:53,815
                    // The difference is our costs are
                    // fixed and posted right there on the door.

                    // Dialogue: 0,0:06:05.81,0:06:07.15,*Default,NTP,0,0,0,,区别在于  我们的收费\N{\fn微软雅黑\fs14}The difference is our costs
                    // Dialogue: 0,0:06:07.15,0:06:09.37,*Default,NTP,0,0,0,,是明码标价挂在门前的\N{\fn微软雅黑\fs14}are fixed and posted right there on the door.
                    transformStarted = true;
                    GetEngTimeStamps(engBlock, ref start, ref end);
                    var ts = GetChsTimeSpan(chsLines[i]);
                    mid = GetTimeMid(start, ts);
                    chsLines[i] = ReplaceTimeStamps(chsLines[i], start, mid);
                    chsLines[i+1] = ReplaceTimeStamps(chsLines[i+1], mid, end);
                    i++;
                    if (!enumerator.MoveNext()) break;
                }
                else if (chsUpper.Contains(engUpper) || engUpper.Contains(chsUpper) ) {
                    transformStarted = true;
                    GetEngTimeStamps(engBlock, ref start, ref end);
                    chsLines[i] = ReplaceTimeStamps(chsLines[i], start, end);
                    if (!enumerator.MoveNext()) break;
                }
                else if (transformStarted) {
                    chsUpper = chsLines[i+1].ToUpper(); 
                    if (chsUpper.Contains(engUpper)) {
                        //this line doesn't exist in eng version, remove it.
                        // Dialogue: 0,0:18:53.52,0:18:54.65,*Default,NTP,0,0,0,,否则他们就是我的\N{\fn微软雅黑\fs14}Then they're mine.
                        // Dialogue: 0,0:19:13.07,0:19:13.44,*Default,NTP,0,0,0,,{\an8\fn方正黑体_GBK\fs18\b1\bord1\shad1\3c&H2F2F2F&}地下83层
                        // Dialogue: 0,0:19:18.60,0:19:20.79,*Default,NTP,0,0,0,,制冷系统已经坏了几周了\N{\fn微软雅黑\fs14}Cooling system's been down for weeks.
                        // 211
                        // 00:18:06,453 --> 00:18:08,307
                        // Then they're mine.

                        // 212
                        // 00:18:30,517 --> 00:18:33,267
                        // Cooling system's been down for weeks.

                        chsLines[i] = string.Empty;
                    }
                    else if (!engUpper.Contains(chsUpper)) {
                        //not in chs lines
                        if (!enumerator.MoveNext()) break;
                        i --; //stay chs cursor
                    }
                    else {
                        Console.WriteLine("something wrong, i=%1, eng=%2, chs=%3", i, engBlock[2], chsLines[i]);
                    }
                }
                engBlock = enumerator.Current;
                i++;
            }
            File.WriteAllLines(resultFile, chsLines);
        }

        private string GetTimeMid(string start, TimeSpan span)
        {
            DateTime dt = DateTime.Parse(start);
            DateTime mid = dt + span;
            return string.Format("{0:H:mm:ss:ff}", mid);
        }

        private bool twoChsInOneBlock(string line1, string line2, List<string> engBlock)
        {
            if (engBlock.Count < 4) return false;
            if (!engBlock[2].ToUpper().Contains(getEngContent(line1).ToUpper())) return false;
            if (!engBlock[2].ToUpper().Contains(getEngContent(line1).ToUpper())) return false;
            return true;
        }

        private string getEngContent(string line)
        {
            var pos = line.IndexOf('}');
            return line.Substring(pos + 1);
        }

        private string ReplaceTimeStamps(string line, string start, string end)
        {
            var items = line.Split(',');
            items[1] = start;
            items[2] = end;
            return string.Join(",", items);
        }

        private TimeSpan GetChsTimeSpan(string line) 
        {
            var parts = line.Split(',');
            DateTime t1 = DateTime.Parse(parts[1]); 
            DateTime t2 = DateTime.Parse(parts[2]); 
            return t2 - t1;
        }

        private void GetEngTimeStamps(List<string> engBlock, ref string start, ref string end)
        {
            var timeItems = engBlock[1].Split(null);
            start= TimeConsolidate(timeItems[0]);
            end = TimeConsolidate(timeItems[2]);
        }
        private string TimeConsolidate(string time) {
            if (time.StartsWith("0")) {
                time = time.Substring(1);
            }
            var pieces = time.Split(',');
            return pieces[0] + "." + pieces[1].Substring(0, 2);
        }

        private IEnumerable<List<string>>  getEnglishBlock() {
            var engLines = System.IO.File.ReadAllLines(this.englishSrc);
            var i = 0; 
            var block = new List<string>();
            while(i < engLines.Length) {
                var line = engLines[i];
                if (line.Length > 0) {
                    block.Add(engLines[i]);
                }
                else {
                    if (!block[2].StartsWith("- <")) yield return block;
                    block = new List<string>();
                }
                i++;
            }

        }
        
    }
}
