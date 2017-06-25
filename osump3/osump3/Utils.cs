using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
        private static string[] SkipDrives = new string[] { @"C:\" };

        public static string SongDir = null;

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


        internal static bool CanRead(string d)
        {
            try
            {
                var readAllow = false;
                var readDeny = false;
                var accessControlList = Directory.GetAccessControl(d);
                if (accessControlList == null)
                    return false;
                var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                if (accessRules == null)
                    return false;

                foreach (FileSystemAccessRule rule in accessRules)
                {
                    if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read) continue;

                    if (rule.AccessControlType == AccessControlType.Allow)
                        readAllow = true;
                    else if (rule.AccessControlType == AccessControlType.Deny)
                        readDeny = true;
                }

                return readAllow && !readDeny;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        internal static bool CanRead(this DirectoryInfo d)
        {
            return CanRead(d.FullName);
        }

        public static string GetSongDir()
        {
            if (SongDir != null)
            {
                if (!new DirectoryInfo(SongDir).Exists) return null;
                return SongDir;
            }

            string ret;

            //method 1
            DirectoryInfo appData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            bool cont = false;
            foreach (string drive in SkipDrives)
            {
                if (appData.FullName.StartsWith(drive))
                {
                    cont = true;
                    break;
                }
            }
            if (!cont)
            {
                if ((ret = GetSongDir(appData)) != null)
                {
                    SongDir = ret;
                    return ret;
                }
            }

            //method 2 - search all drives
            foreach (DriveInfo drive in GetDrives())
            {
                try
                {
                    DirectoryInfo usrDir = new DirectoryInfo(drive.Name + "Users");
                    if (usrDir.Exists)
                    {
                        foreach (DirectoryInfo d in usrDir.GetDirectories())
                        {
                            if ((ret = GetSongDir(new DirectoryInfo(d.FullName + @"\AppData\Local"))) != null)
                            {
                                SongDir = ret;
                                return ret;
                            }
                        }
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            return null;
        }

        private static string GetSongDir(DirectoryInfo appData)
        {
            try
            {
                if (!appData.Exists) return null;
                string d = appData.FullName + @"\osu!\Songs";
                if (Directory.Exists(d)) return d;
            }
            catch { }
            return null;
        }

        private static List<DriveInfo> GetDrives()
        {
            List<DriveInfo> fixedready = new List<DriveInfo>();
            List<DriveInfo> ready = new List<DriveInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (SkipDrives.Contains(drive.Name)) continue;
                if (drive.IsReady)
                {
                    if (drive.DriveType == DriveType.Fixed) fixedready.Add(drive);
                    else ready.Add(drive);
                }
            }

            List<DriveInfo> ret = new List<DriveInfo>();
            ret.AddRange(fixedready);
            ret.AddRange(ready);
            return ret;
        }
    }
}
