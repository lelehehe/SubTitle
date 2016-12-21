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
                if (chsLines[i].ToUpper().Contains(engBlock[2].ToUpper())) {
                    transformStarted = true;
                    GetTimeStamps(engBlock, ref start, ref end);
                    chsLines[i] = ReplaceTimeStamps(chsLines[i], start, end);
                    if (!enumerator.MoveNext()) break;
                    engBlock = enumerator.Current;
                    //Console.WriteLine(chsLines[i]);
                }
                else if (i < chsLines.Length - 1 && twoChsInOneBlock(chsLines[i], chsLines[i+1], engBlock)) {
                    //62
                    // 00:05:49,912 --> 00:05:53,815
                    // The difference is our costs are
                    // fixed and posted right there on the door.

                    // Dialogue: 0,0:06:05.81,0:06:07.15,*Default,NTP,0,0,0,,区别在于  我们的收费\N{\fn微软雅黑\fs14}The difference is our costs
                    // Dialogue: 0,0:06:07.15,0:06:09.37,*Default,NTP,0,0,0,,是明码标价挂在门前的\N{\fn微软雅黑\fs14}are fixed and posted right there on the door.
                    transformStarted = true;
                    GetTimeStamps(engBlock, ref start, ref end);
                    GetTimeSpan(chsLines[i], span);
                    mid = GetTimeMid(start, span);
                    chsLines[i] = ReplaceTimeStamps(chsLines[i], start, mid);
                    chsLines[i+1] = ReplaceTimeStamps(chsLines[i+1], mid, end);
                    i++;
                }
                else if (transformStarted) {
                    Console.WriteLine(chsLines[i]);
                    Console.WriteLine(engBlock[2]);
                }
                i++;
            }
            File.WriteAllLines(resultFile, chsLines);
        }

        private string GetTimeMid(string start, float span)
        {
            throw new NotImplementedException();
        }

        private void GetTimeSpan(string v, float span)
        {
            throw new NotImplementedException();
        }

        private bool twoChsInOneBlock(string line1, string line2, List<string> engBlock)
        {
            if (engBlock.Count < 4) return false;
            if (!engBlock[2].ToUpper().Contains(getEngContent(line1))) return false;
            if (!engBlock[2].ToUpper().Contains(getEngContent(line1))) return false;
            return true;
        }

        private string getEngContent(string line)
        {
            var pos = line.IndexOf('}');
            return line.Substring(pos + 1);
        }

        private string ReplaceTimeStamps(string line, string start, string end)
        {
            var pos = line.IndexOf(',');
            var length = 21;
            return line.Substring(0, pos) + start + "," + end + line.Substring(pos + length);
        }

        private void GetTimeStamps(List<string> engBlock, ref string start, ref string end)
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
