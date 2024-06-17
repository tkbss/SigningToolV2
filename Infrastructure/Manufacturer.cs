using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Manufacturer
    {
        public static string[] manu = { "WINCOR", "DIEBOLD", "NCR", "GLORY", "MVS", "MSIX","PREMA","SUZOH", "MVSDN" };
        public static string[] manu_abr = { "MVS", "SIX", "NCR", "WNI", "GGS", "DSB", "MSIX","PRE","SUZ", "MDN" };
        public static Dictionary<string, string> ManuAbrMapping = new Dictionary<string, string>() {{ "MVS", "MVS" },
        {"SIX","MSIX"}, {"NCR","NCR"}, {"WINCOR","WNI"}, {"GLORY","GGS" }, {"DIEBOLD","DSB" }, {"MSIX","SIX" },{"PREMA","PRE"},{"SUZOH","SUZ" },{"MVSDN","MDN" } };
    }
}
