using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static Regex SectionRegex = new Regex(@"\[("+Utils.OSU_SECTIONS+@")\](\r\n?|\n)(([^\n\r]+)((\r\n?|\n)|$))+");
        public FileInfo File;

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
                if (!this.ParseSection(section.Value))
                {
                    Console.WriteLine("Error parsing section: " + section.Groups[1]);
                }
            }
            return this;
        }

        private Char ReadNot(this StringReader sr, char expect)
        {
            if((Char)sr.Read()!=expect)
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

        public FileInfo AudioFilename;

        private bool ParseGeneral(StringReader sr)
        {
            while (true)
            {
                StringBuilder nraw = new StringBuilder();
                char c;
                while ((c = (Char)sr.Read()) != ':') nraw.Append(c);
                if (nraw != null)
                {
                    switch (nraw.ToString().Trim())
                    {
                        case "AudioFilename":
                            AudioFilename = new FileInfo(this.File.Directory.FullName + @"\" + sr.ReadLine());
                            break;
                        default:
                            Console.WriteLine("Cannot handle: " + nraw.ToString().Trim());
                            sr.ReadLine();
                            break;
                    }
                }
                else { break; }
            }
            return true;
        }

        private bool ParseMetadata(StringReader sr)
        {
            return false;
        }

        private bool ParseDifficulty(StringReader sr)
        {
            return false;
        }

        private bool ParseEvents(StringReader sr)
        {
            return false;
        }
    }
}
