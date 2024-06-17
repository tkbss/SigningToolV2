namespace Infrastructure
{
    public class PackageInfo
    {
        public string FileName { get; set; }
        public string Vendor { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Date { get; set; }

        public string ExtractionPath { get; set; }

        public List<string> Executables { get; set; }
        public List<SecurityInfo> Security { get; set; }
    }
    public class SecurityInfo
    {
        public string FileName { get; set; }
        public string Algorithm { get; set; }
        public string Digest { get; set; }

        public string ComputedDigest { get; set; }
    }
}
