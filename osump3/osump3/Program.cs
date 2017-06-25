using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osump3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Utils.GetSongDir());

            OsuFile osu = new OsuFile(new System.IO.FileInfo(@"D:\Users\syanochara\AppData\Local\osu!\Songs\59936 Hanazawa Kana ft Snoop Dogg - Weed Circulation\Hanazawa Kana ft. Snoop Dogg - Weed Circulation (Milkshake) [and those roll the best].osu")).Parse();
            Console.WriteLine(osu);
        }
    }
}
