using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osump3
{
    public static class Utils
    {
        //public const string OSU_SECTIONS = "General|Editor|Metadata|Difficulty|Events|TimingPoints|Colours|HitObjects";
        public const string OSU_SECTIONS = "General|Metadata|Difficulty|Events";
        public const string IMAGE_TYPES = "bmp|gif|jpeg|jpg|png|tiff";
        
        public enum ImageResolveMode {First};

        public static bool Debug = true;
        public static bool AsciiOnly = false;
        public static OsuFile.ModeType Modes = OsuFile.ModeType.Osu | OsuFile.ModeType.Taiko | OsuFile.ModeType.CatchTheBeat | OsuFile.ModeType.OsuMania;
        public static ImageResolveMode ResolveMode = ImageResolveMode.First;

        public static void Quit(string message="")
        {
            //TODO
        }

        internal static Char ReadNot(this StringReader sr, char not)
        {
            int c;
            if ((Char)(c = sr.Read()) != not)
            {
                if (c == -1) return not;
            }
            return (Char)c;
        }

        internal static string ReadUntil(this StringReader sr, char until)
        {
            StringBuilder sraw = new StringBuilder();
            char c;
            while ((c = sr.ReadNot(until)) != until) sraw.Append(c);
            return sraw.ToString().Trim();
        }

        internal static string ReadString(this StringReader sr, uint look=0)
        {
            string s;
            if (look == 0)
            {
                s = sr.ReadLine();
            }
            else
            {
                StringBuilder sraw = new StringBuilder();
                for (int i = 0; i < look; i++) sraw.Append((Char)sr.Read());
                s = sraw.ToString();
            }
            return s.Trim();
        }

        internal static bool ReadBool(this StringReader sr, uint look = 0)
        {
            return "1".Equals(sr.ReadString(look));
        }

        internal static int ReadInt(this StringReader sr, uint look = 0)
        {
            return Int32.Parse(sr.ReadString(look));
        }

        internal static FileInfo ReadFile(this StringReader sr, FileInfo osuFile, uint look = 0)
        {
            return new FileInfo(osuFile.Directory.FullName + @"\" + sr.ReadString(look));
        }

        internal static float ReadFloat(this StringReader sr, uint look = 0)
        {
            return Single.Parse(sr.ReadString(look));
        }

        internal static T ReadEnum<T>(this StringReader sr, uint look = 0)
        {
            return (T)(Object)sr.ReadInt(look);
        }
    }
}
