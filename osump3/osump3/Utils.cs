using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osump3
{
    public static class Utils
    {
        //public const string OSU_SECTIONS = "General|Editor|Metadata|Difficulty|Events|TimingPoints|Colours|HitObjects";
        public const string OSU_SECTIONS = "General|Metadata|Difficulty|Events";

        public static bool Debug = true;
        public static bool AsciiOnly = false;

        public static void Quit(string message="")
        {
            //TODO
        }
    }
}
