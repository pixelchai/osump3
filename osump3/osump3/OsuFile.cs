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
        public enum SectionType { Unkown, General, Editor, Metadata, Difficulty, Events, TimingPoints, Colours, HitObjects }
        public enum ModeType { Osu,Taiko,CatchTheBeat,OsuMania};

        public static Regex SectionRegex = new Regex(@"\[("+Utils.OSU_SECTIONS+@")\](\r\n?|\n)(([^\n\r]+)((\r\n?|\n)|$))+");
        public static Regex ImagefileQuotesRegex = new Regex("\\\"([^\\x00-\\x1f\\\\?*:\\\";|/]+\\.(" + Utils.IMAGE_TYPES + "))\\\"");
        public FileInfo File;

        public FileInfo AudioFilename;
        public int AudioLeadIn;
        public int PreviewTime;
        public bool Countdown;
        public string SampleSet;
        public float StackLeniency;
        public ModeType Mode;
        public bool LetterboxInBreaks;
        public bool WidescreenStoryboard;

        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicode;
        public string Creator;
        public string Version;
        public string Source;
        public string Tags;
        public int BeatmapID;
        public int BeatmapSetID;

        public float HPDrainRate;
        public float CircleSize;
        public float OverallDifficulty;
        public float ApproachRate;
        public float SliderMultiplier;
        public float SliderTickRate;

        public FileInfo Image;

        public OsuFile(FileInfo file)
        {
            this.File = file;
        }

        public OsuFile Parse()
        {
            string raw;
            using (FileStream fs = new FileStream(File.FullName, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                raw = sr.ReadToEnd();
            }

            foreach (Match section in SectionRegex.Matches(raw))
            {
                if (!this.ParseSection(section.Value))
                {
                    Console.WriteLine("Error parsing section: " + section.Groups[1]);
                }
            }
            return this;
        }

        private bool ParseSection(string section)
        {
            using (StringReader sr = new StringReader(section))
            {
                if (sr.Read() != '[') return false;
                SectionType stype = (SectionType)Enum.Parse(typeof(SectionType), sr.ReadUntil(']'), true);
                switch (stype)
                {
                    case SectionType.General: return ParseGeneral(sr);
                    case SectionType.Metadata: return ParseMetadata(sr);
                    case SectionType.Difficulty: return ParseDifficulty(sr);
                    case SectionType.Events: return ParseEvents(sr);
                    case SectionType.Unkown: default: return false;
                }
            }
        }

        private bool ParseGeneral(StringReader sr)
        {
            while (true)
            {
                string nraw = sr.ReadUntil(':');
                switch (nraw)
                {
                    case "AudioFilename":
                        AudioFilename = sr.ReadFile(this.File);
                        break;
                    case "AudioLeadIn":
                        AudioLeadIn = sr.ReadInt();
                        break;
                    case "PreviewTime":
                        PreviewTime = sr.ReadInt();
                        break;
                    case "Countdown":
                        Countdown = sr.ReadBool();
                        break;
                    case "SampleSet":
                        SampleSet = sr.ReadString();
                        break;
                    case "StackLeniency":
                        StackLeniency = sr.ReadFloat();
                        break;
                    case "Mode":
                        Mode = sr.ReadEnum<ModeType>();
                        break;
                    case "LetterboxInBreaks":
                        LetterboxInBreaks= sr.ReadBool();
                        break;
                    case "WidescreenStoryboard":
                        WidescreenStoryboard= sr.ReadBool();
                        break;
                    case "":
                        return true;
                    default:
                        Console.WriteLine("Cannot handle: " + nraw.ToString().Trim());
                        sr.ReadLine();
                        break;
                }
            }
        }

        

        private bool ParseMetadata(StringReader sr)
        { 
            while (true)
            {
                string nraw = sr.ReadUntil(':');
                switch (nraw)
                {
                    case "Title":
                        Title = sr.ReadString();
                        break;
                    case "TitleUnicode":
                        TitleUnicode = sr.ReadString();
                        break;
                    case "Artist":
                        Artist = sr.ReadString();
                        break;
                    case "ArtistUnicode":
                        ArtistUnicode = sr.ReadString();
                        break;
                    case "Creator":
                        Creator = sr.ReadString();
                        break;
                    case "Version":
                        Version = sr.ReadString();
                        break;
                    case "Source":
                        Source = sr.ReadString();
                        break;
                    case "Tags":
                        Tags = sr.ReadString();
                        break;
                    case "BeatmapID":
                        BeatmapID = sr.ReadInt();
                        break;
                    case "BeatmapSetID":
                        BeatmapSetID = sr.ReadInt();
                        break;
                    case "":
                        return true;
                    default:
                        Console.WriteLine("Cannot handle: " + nraw.ToString().Trim());
                        sr.ReadLine();
                        break;
                }
            }
        }

        private bool ParseDifficulty(StringReader sr)
        {
            while (true)
            {
                string nraw = sr.ReadUntil(':');
                switch (nraw)
                {
                    case "HPDrainRate":
                        HPDrainRate = sr.ReadFloat();
                        break;
                    case "CircleSize":
                        CircleSize = sr.ReadFloat();
                        break;
                    case "OverallDifficulty":
                        OverallDifficulty = sr.ReadFloat();
                        break;
                    case "ApproachRate":
                        ApproachRate = sr.ReadFloat();
                        break;
                    case "SliderMultiplier":
                        SliderMultiplier = sr.ReadFloat();
                        break;
                    case "SliderTickRate":
                        SliderTickRate = sr.ReadFloat();
                        break;
                    case "":
                        return true;
                    default:
                        Console.WriteLine("Cannot handle: " + nraw.ToString().Trim());
                        sr.ReadLine();
                        break;
                }
            }
        }

        private bool ParseEvents(StringReader sr)
        {
            string all = sr.ReadToEnd();
            List<FileInfo> images = new List<FileInfo>();
            foreach (Match m in ImagefileQuotesRegex.Matches(all))
            {
                images.Add(new FileInfo(this.File.Directory.FullName + @"\" + m.Groups[1]));
            }
            if (images.Count > 0)
            {
                switch (Utils.ResolveMode)
                {
                    case Utils.ImageResolveMode.First:
                        Image = images[0];
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                Console.WriteLine("No image found!");
            }
            return true;
        }
    }
}
