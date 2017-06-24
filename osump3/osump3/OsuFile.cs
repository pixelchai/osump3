using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace osump3
{
    public class OsuFile
    {
        public enum Section { Unkown, General, Editor, Metadata, Difficulty, Events, TimingPoints, Colours, HitObjects }

        public static Regex SectionRegex = new Regex("\\[(" + Utils.OSU_SECTIONS + "+)\\]\\n(([^\\n]+)(\\s|$))+");
        public FileInfo File = null;

        public OsuFile(FileInfo file)
        {
            this.File = file;
        }

        public OsuFile Parse()
        {
            string raw;
            #region read
            using (FileStream fs = new FileStream(File.FullName, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                raw = sr.ReadToEnd();
            }
            #endregion

            foreach (Match section in SectionRegex.Matches(raw))
            {
                this.ParseSection(section.Value);
            }
            return this;
        }

        private bool ParseSection(string section)
        {
            using (StringReader sr = new StringReader(section))
            {
                if (sr.Read() != '[') return false;
                StringBuilder sraw = new StringBuilder();
                char c;
                while ((c = (Char)sr.Read()) != ']') sraw.Append(c);
                Section stype = (Section)Enum.Parse(typeof(Section), sraw.ToString(), true);
                switch (stype)
                {
                    case Section.General: return ParseGeneral(sr);
                    case Section.Metadata: return ParseMetadata(sr);
                    case Section.Difficulty: return ParseDifficulty(sr);
                    case Section.Events: return ParseEvents(sr);
                    case Section.Unkown: default: return false;
                }
            }
        }

        private bool ParseGeneral(StringReader sr)
        {
            while (true)
            {
                StringBuilder nraw = new StringBuilder();
                char c;
                while ((c = (Char)sr.Read()) != ':') nraw.Append(c);

            }
        }

        private bool ParseMetadata(StringReader sr)
        {
        }

        private bool ParseDifficulty(StringReader sr)
        {
        }

        private bool ParseEvents(StringReader sr)
        {
        }
    }
}
