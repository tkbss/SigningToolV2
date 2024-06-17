using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class OIDs
    {
        public static string SHA1
        {
            get { return "1.3.14.3.2.26"; }
        }
        public static string SHA256
        {
            get { return "2.16.840.1.101.3.4.2.1"; }
        }
        public static string RSAwithSHA1
        {
            get { return "1.2.840.113549.1.1.5"; }
        }
        public static string RSAwithSHA256
        {
            get { return "1.2.840.113549.1.1.11"; }
        }
    }
}
