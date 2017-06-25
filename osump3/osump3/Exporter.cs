using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace osump3
{
    public class Exporter
    {
        public DirectoryInfo SongDir;
        public DirectoryInfo ExportDir;

        public Exporter(string path)
        {
            SongDir = new DirectoryInfo(path);
            ExportDir = new DirectoryInfo(Utils.ExportPath);
            if (!ExportDir.Exists) ExportDir.Create();
        }

        public void Export()
        {
            foreach (DirectoryInfo dir in SongDir.GetDirectories())
            {
                FileInfo osuFile = GetOsuFile(dir);
                if (osuFile != null)
                {
                    ExportOsuFile(new OsuFile(osuFile).Parse());
                }
            }
        }

        internal void ExportOsuFile(OsuFile osu)
        {
            if (osu.Mode.HasFlag(osu.Mode) && osu.AudioFilename != null)
            {
                if (osu.AudioFilename.Extension == ".mp3")
                {
                    FileInfo f = osu.AudioFilename.CopyTo(ConstructPath(osu.Title+osu.AudioFilename.Extension), true);
                    TagLib.File file = TagLib.File.Create(f.FullName);

                    file.Tag.Album = "Osu - "+osu.Creator;
                    file.Tag.Comment = osu.Tags;
                    file.Tag.Performers = new string[] { osu.Creator };
                    file.Tag.AlbumArtists = new string[] { (Utils.AsciiOnly?osu.Artist:osu.ArtistUnicode) };
                    file.Tag.Title = (Utils.AsciiOnly ? osu.Title : osu.TitleUnicode);
                    file.Tag.Genres = new string[] { "Osu" };
                    using (MemoryStream ims = new MemoryStream())
                    {
                        if (osu.Image != null)
                        {
                            Image img;
                            try
                            {
                                img = Utils.ResizeImage(Image.FromFile(osu.Image.FullName), new Size(6000, 6000));
                            }
                            catch
                            {
                                img = Image.FromFile(osu.Image.FullName);
                            }

                            TagLib.Picture pic = new TagLib.Picture();
                            pic.Type = TagLib.PictureType.FrontCover;
                            pic.Description = osu.Image.Name;

                            pic.MimeType = MediaTypeNames.Image.Jpeg;
                            img.Save(ims, ImageFormat.Jpeg);
                            ims.Position = 0;
                            pic.Data = TagLib.ByteVector.FromStream(ims);
                            file.Tag.Pictures = new TagLib.IPicture[] { pic };
                        }

                        file.Save();
                        Console.WriteLine(file.Name);
                    }
                }
                else
                {
                    if (!Utils.OnlyMp3)
                    {
                        FileInfo f = osu.AudioFilename.CopyTo(ConstructPath(osu.AudioFilename.Name), true);
                    }
                }
            }
        }

        private string ConstructPath(string filename)
        {
            return ExportDir.FullName + @"\" + filename;
        }


        internal FileInfo GetOsuFile(DirectoryInfo dir)
        {
            foreach (FileInfo f in dir.GetFiles())
            {
                if (f.Extension == ".osu") return f;
            }
            return null;
        }
    }
}
