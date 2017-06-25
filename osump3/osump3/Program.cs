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
            new Exporter(Utils.GetSongDir()).Export();
        }
    }
}
