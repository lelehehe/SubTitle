using System.Collections.Generic;
using System;
using System.IO;

namespace SubTitleApp
{
    public class SubTitleFile
    {
        public string englishSrc { get; set; }
        public string chineseSrc { get; set; }

        public SubTitleFile(string engSrc, string chsSrc)
        {
            englishSrc = engSrc;
            chineseSrc = chsSrc;
        }
        public void Process() {
            foreach(var block in this.getEnglishBlock()) {
                Console.Write(block.Count);
            }
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
                    yield return block;
                    block = new List<string>();
                }
                i++;
            }

        }
        
    }
}
