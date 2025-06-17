using Infrastructure.Exceptions;
using NavigationModule.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using Unity;
//using static System.Net.WebRequestMethods;


namespace Infrastructure
{

    public class PackageProcessing 
    {
       
        string root_dir = @"ATMSoftwareSigning\Packages";
        char[] fn_seperator = { '_', '_' };
        char[] version_seperator = { '-' };
        
       
        string xml_security_p = @"/infodata/security/filesecurity";
        string xml_security = @"/infodata/security";
        string xml_package_info = @"/infodata/package";
        public PackageProcessing()
        {
           

        }
        public string Error { get; set; }
        
        public bool PackageSigFileExists(MANUFACTURER m,ENVIROMENT e, SIGNER s,STORETYPE st, string version,string package_name)
        {
            string f_p = GetVersionExtractionPath(Converter.Manu(m), s, st,e,version,package_name);
            if (s == SIGNER.MANU)
            {
                string env = Converter.Env(e);
                string q = env + ".sign.export";                
                if (Directory.GetFiles(f_p).Where(p=>p.Contains(q)).Count()>0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (Directory.GetFiles(f_p).Count(p => Path.GetExtension(p) == ".export") > 0)
                    return true;
                else
                    return false;
            }
        }
        public void RemovePackageSignature(PackageInfo pi)
        {
            string p = pi.ExtractionPath;
            if (Directory.Exists(p) == false)
                return;
            foreach(string fn in Directory.GetFiles(p,"*.export"))
            {
                File.Delete(fn);
            }
        }
        public void RemovePackage(MANUFACTURER m, SIGNER s,STORETYPE st,ENVIROMENT e, string version,string package_name)
        {
            string f_p = GetVersionExtractionPath(Converter.Manu(m), s,st,e, version,package_name);
            if (Directory.Exists(f_p) == true)
            {
                try
                {
                    Directory.Delete(f_p, true);
                }
                catch { }
            }
            string v_p= GetVersionPath(Converter.Manu(m), s, st,e, version);           
            if (Directory.Exists(v_p)==true)
            {
                string[] dirEntries = Directory.GetDirectories(v_p);
                if (dirEntries==null || dirEntries.Length==0)
                {
                    Directory.Delete(v_p);
                }
            }
        }
        public async Task ExportPackage(MANUFACTURER m,ENVIROMENT e,SIGNER s,STORETYPE st,string version,string package_name,string target_path)
        {
            string f_p = GetVersionExtractionPath(Converter.Manu(m), s,st,e, version,package_name);
            if (Directory.GetFiles(f_p).Length == 0)
                return;
            //CopyToExportDir(f_p,s,Converter.Env(e));
            string exportFilename = string.Empty;
            var filesToZip=new List<string>();
            
            foreach (string fp in Directory.GetFiles(f_p))
            {
                string dest_fn = string.Empty;
                if (Path.GetExtension(fp) == ".sign")
                {
                    string t = fp+ ".tmp";
                    File.Move(fp, t,true);
                    continue;
                }
                if (Path.GetExtension(fp) == ".export")
                {
                    exportFilename = fp;
                    string ex = Path.GetFileNameWithoutExtension(fp);
                    while (Path.HasExtension(ex) == true)
                    {
                        ex = Path.GetFileNameWithoutExtension(ex);
                    }
                    ex = Path.Combine(f_p,ex + ".sign");
                    File.Copy(fp, ex,true);
                    filesToZip.Add(ex);
                }
                else
                {
                    filesToZip.Add(fp);
                }
                
            }
            try
            {
                string fn = Path.GetFileNameWithoutExtension(Directory.GetFiles(f_p).First(p => Path.GetExtension(p) == ".info"));
                string z_f = Path.Combine(f_p, fn + ".zip");
                await CreateZipFromFilesAsync(filesToZip, z_f);
                File.Move(z_f, Path.Combine(target_path, fn + ".zip"), true);
                string signFile = Directory.GetFiles(f_p).FirstOrDefault(p => Path.GetExtension(p) == ".sign");
                if (signFile != null)
                {
                    File.Delete(signFile);
                }
                string moveFile = Directory.GetFiles(f_p).FirstOrDefault(p => Path.GetExtension(p) == ".tmp");
                if (moveFile != null)
                {
                    string targetP = Path.Combine(f_p,Path.GetFileNameWithoutExtension(moveFile));
                    File.Move(moveFile, targetP, true);
                }
            }
            catch(Exception ex)
            {
                Error = ex.Message;
            }
        }
        private  async Task CreateZipFromFilesAsync(IEnumerable<string> filePaths, string destinationZipFilePath)
        {
            await Task.Run(() =>
            {
                using (var zipArchive = ZipFile.Open(destinationZipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                        }
                    }
                }
            });
        }
        public void ExportATMPackage(MANUFACTURER m, ENVIROMENT e, SIGNER s, STORETYPE st, string version, string package_name, string target_path)
        {
            string f_p = GetVersionExtractionPath(Converter.Manu(m), s, st,e, version, package_name);
            if (Directory.GetFiles(f_p).Count() == 0)
                return;
            CopyToExportDir(f_p, SIGNER.ATM, Converter.Env(e));
            string fn = Path.GetFileNameWithoutExtension(Directory.GetFiles(f_p).First(p => Path.GetExtension(p) == ".info"));
            string z_f = Path.Combine(target_path, fn + ".zip");
            string export_dir = Path.Combine(f_p, "EXPORT");
            if (File.Exists(z_f) == true)
                File.Delete(z_f);
            ZipFile.CreateFromDirectory(export_dir, z_f);
        }
        public async Task CopyPackageAsync(MANUFACTURER m, ENVIROMENT e, SIGNER s, STORETYPE st, string version, string package_name,
            ENVIROMENT targetENV,SIGNER targetS)
        {
            string sourcePath= GetVersionExtractionPath(Converter.Manu(m), s,  st,  e,  version,  package_name);
            string targetPath = GetVersionExtractionPath(Converter.Manu(m), targetS,  st,  targetENV,  version,  package_name);
            if (Directory.Exists(targetPath) == true)
                Directory.Delete(targetPath, true);
            Directory.CreateDirectory(targetPath);
            foreach (string fp in Directory.GetFiles(sourcePath))
            {
                string dest_fn = string.Empty;
                if (Path.GetExtension(fp) == ".sign")
                    continue;
                if (Path.GetExtension(fp) == ".export")
                {                    
                    string fn = Path.GetFileNameWithoutExtension(fp);
                    while (Path.HasExtension(fn) == true)
                    {
                        fn = Path.GetFileNameWithoutExtension(fn);
                    }
                    fn = fn + ".sign";
                    dest_fn = Path.Combine(targetPath, fn);
                }
                else
                {
                    string fn = Path.GetFileName(fp);
                    dest_fn = Path.Combine(targetPath, fn);
                }                
                await CopyFileAsync(fp, dest_fn);
            }

        }
        private void CopyToExportDir(string f_p,SIGNER s,string env)
        {
                      
            string z_p = Path.Combine(f_p, "EXPORT");
            if (Directory.Exists(z_p) == true)
                Directory.Delete(z_p, true);
            Directory.CreateDirectory(z_p);
            foreach (string fp in Directory.GetFiles(f_p))
            {
                string dest_fn = string.Empty;
                if (Path.GetExtension(fp) == ".sign")
                    continue;
                if (Path.GetExtension(fp) == ".export")
                {
                    if (CopySignatureFile(fp, s, env) == false)
                        continue;
                    string fn = Path.GetFileNameWithoutExtension(fp);
                    while(Path.HasExtension(fn)==true)
                    {                        
                        fn= Path.GetFileNameWithoutExtension(fn);
                    }
                    fn = fn + ".sign";
                    dest_fn = Path.Combine(z_p, fn);
                }
                else
                {
                    string fn = Path.GetFileName(fp);
                    dest_fn = Path.Combine(z_p, fn);
                }
                if (File.Exists(dest_fn))
                    File.Delete(dest_fn);
                File.Copy(fp, dest_fn);
            }
        }
        private bool CopySignatureFile(string fp,SIGNER s,string env)
        {
            if (s == SIGNER.MANU)
            {
                string fn = Path.GetFileNameWithoutExtension(fp);
                string e = "." + env;
                while (Path.HasExtension(fn) == true)
                {
                    string ext = Path.GetExtension(fn);
                    if (ext == e)
                        return true;
                    else
                        fn = Path.GetFileNameWithoutExtension(fn);
                }
                return false;
            }
            else
            {
                if (s == SIGNER.ATM || s == SIGNER.ATM_DEVICE)
                    return true;
                if (fp.Contains("PROD") || fp.Contains("TEST"))
                    return false;
                else
                    return true;
            }                
        }
        public void MakeSetupInfo(MANUFACTURER m, SIGNER s, STORETYPE st,ENVIROMENT e, string version,string package_name)
        {
            //string f_p = GetVersionExtractionPath(Converter.Manu(m), s,st,e, version,package_name);
            //RemoveSecurityNode(f_p);
            //AddSecurityInfo(f_p);
        }
        public string GetVersionExtractionPath(string manu, SIGNER signer,STORETYPE st,ENVIROMENT e, string version,string package_name)
        {
            if (string.IsNullOrEmpty(package_name) == true)
                return string.Empty;
            string ep = GetExtractionPath(manu, signer,st,e);
            string v_p = Path.Combine(ep, version);
            if (Directory.Exists(v_p) == false)
            {
                Directory.CreateDirectory(v_p);
            }
            string p_n = Path.Combine(v_p, package_name);
            if (Directory.Exists(p_n) == false)
            {
                Directory.CreateDirectory(p_n); 
            }
            return p_n;
        }
        public string GetVersionPath(string manu, SIGNER signer, STORETYPE st,ENVIROMENT e, string version)
        {
            string ep = GetExtractionPath(manu, signer, st,e);
            string v_p = Path.Combine(ep, version);
            return v_p;
        }
        public string GetPackageFileName(string manu, SIGNER signer, STORETYPE st,ENVIROMENT e, string version, string package_name)
        {
            string f_p = GetVersionExtractionPath(manu, signer,st,e, version, package_name);
            if (Directory.GetFiles(f_p).Count() == 0)
                return string.Empty;
            string fn = Path.GetFileNameWithoutExtension(Directory.GetFiles(f_p).First(p => Path.GetExtension(p) == ".info"));
            return fn;
        }
        public string GetExtractionPath(string manu,SIGNER signer,STORETYPE st,ENVIROMENT e)
        {
            string app_data = @"c:\Users\All Users\";//System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments);
            string root_path = Path.Combine(app_data, root_dir);
            string env_path = root_path;
            if (Directory.Exists(root_path) == false)
            {
                Directory.CreateDirectory(root_path);
            }          
            string manu_path = Path.Combine(env_path, manu);
            if (Directory.Exists(manu_path) == false)
            {
                Directory.CreateDirectory(manu_path);
            }
            string s_p = Path.Combine(manu_path, Converter.Signer(signer));
            if (Directory.Exists(s_p) == false)
            {
                Directory.CreateDirectory(s_p);
            }
            if(signer!=SIGNER.MANU)
            {
                s_p = Path.Combine(s_p, Converter.ST(st));
                if (Directory.Exists(s_p) == false)
                {
                    Directory.CreateDirectory(s_p);
                }
                if(st==STORETYPE.KMS && signer==SIGNER.ATM)
                {
                    s_p = Path.Combine(s_p, Converter.Env(e));
                    if (Directory.Exists(s_p) == false)
                    {
                        Directory.CreateDirectory(s_p);
                    }
                }
                if (st == STORETYPE.KMS && signer == SIGNER.ATM_DEVICE)
                {
                    s_p = Path.Combine(s_p, Converter.Env(e));
                    if (Directory.Exists(s_p) == false)
                    {
                        Directory.CreateDirectory(s_p);
                    }
                }
            }
            return s_p;
        }
        public List<string> ExtractedVersions(string extraction_path)
        {
            List<string> versions = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(extraction_path))
                {
                    string[] dirs=d.Split(Path.DirectorySeparatorChar);
                    if (dirs.Count() > 0)
                    {                        
                        versions.Add(dirs[dirs.Length-1]);
                    }
                }
            }
            catch
            {
                
            }
            return versions;
        }
        public List<string> GetPackageNameList(string path,string version)
        {
            string version_path = Path.Combine(path, version);
            List<string> packages = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(version_path))
                {
                    string[] dirs = d.Split(Path.DirectorySeparatorChar);
                    if (dirs.Count() > 0)
                    {
                        packages.Add(dirs[dirs.Length - 1]);
                    }
                }
            }
            catch { }
            return packages;

        }
        public PackageInfo ReadPackageInfo(string extraction_path, string package_provider, IUnityContainer container)
        {
            PackageInfo pi = new PackageInfo();
            string[] files = Directory.GetFiles(extraction_path);
            if (files.Count() == 0) 
            {
                if (Directory.Exists(extraction_path) == true)
                {
                    try
                    {
                        Directory.Delete(extraction_path, true);
                    }
                    catch { }
                }
                throw new PackageProcessingException("No package files in extraction path: " + extraction_path); 
            }
            string fn = files[0];
            foreach(string f in files)
            {
                if(Path.GetExtension(f)==".info")
                {
                    fn = f;
                    break;
                }
            }
            try
            {
                ValidateFileName(fn,  pi);
            }
            catch
            {
                throw new PackageProcessingException("Incorrect package name");
            }
            pi.ExtractionPath = extraction_path;
            pi.Executables = new List<string>();
            foreach (string entry in Directory.GetFiles(pi.ExtractionPath))
            {
                string f = Path.GetFileNameWithoutExtension(entry);
                //if (f != pi.FileName)
                //    continue;
                if (Path.GetExtension(entry) == ".sign")
                    continue;
                if (Path.GetExtension(entry) == ".info")
                    continue;
                if (Path.GetExtension(entry) == ".export")
                    continue;
                pi.Executables.Add(entry);
            }
            ParseSetupInfoAsync(pi,container);
            return pi;

        }
        
        
       
        
        public bool CheckSetupInfo(PackageInfo pi)
        {
            XmlDocument doc = new XmlDocument();
            string info_path = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            doc.Load(info_path);
            XmlNodeList nodes = doc.DocumentElement.SelectNodes(xml_package_info);           
            foreach (XmlNode node in nodes)
            {
                string m=node.SelectSingleNode("vendor").InnerText;
                if(m!=pi.Vendor)
                {
                    Error = "Vendor in setup info corresponds not with file name";
                    return false;
                }
                string n= node.SelectSingleNode("name").InnerText;
                if(n!=pi.Name)
                {
                    Error = "Package Name in setup info corresponds not with file name";
                    return false;
                }
                string v= node.SelectSingleNode("version").InnerText;
                if(v!=pi.Version)
                {
                    Error = "Version in setup info corresponds not with file name";
                    return false;
                }
                string d= node.SelectSingleNode("date").InnerText;
                d=d.Replace(".", string.Empty);
                if (d != pi.Date)
                {
                    //Error = "Date in setup info corresponds not with file name";
                    //return false;
                }
                break;
            }
            return true;
        }
        public void ParseSetupInfoAsync(PackageInfo pi, IUnityContainer container)
        {
            var mpm = container.Resolve<PackageManagementModel>();
            pi.Security = new List<SecurityInfo>();
            PackageModel p = null;
            var manu=mpm.ManuPackages.FirstOrDefault(m => Converter.Abrevation(m.Manufacturer) == pi.Vendor);
            if ((manu!=null))
            {
                var v = manu.VersionList.FirstOrDefault(v => v.Version == pi.Version);
                if ((v!=null))
                {
                    p = v.PackageNameList.FirstOrDefault(pn => pn.PackageName == pi.Name);
                    if(p.DigestList==null)
                    {
                        p.DigestList = new List<string>();
                    }
                }                
            }
            
            
            XmlDocument doc = new XmlDocument();
            string info_path = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            doc.Load(info_path);
            XmlNodeList nodes = doc.DocumentElement.SelectNodes(xml_security_p);
            SecurityProcessing s = new SecurityProcessing();
            int NodeCount = 0;
            foreach (XmlNode node in nodes)
            {                
                SecurityInfo si = new SecurityInfo();
                si.FileName = node.SelectSingleNode("filename").InnerText;                
                si.Digest = node.SelectSingleNode("digest").InnerText;
                si.Algorithm = node.SelectSingleNode("algorithm").InnerText;
                if(si.Algorithm!= "2.16.840.1.101.3.4.2.1")
                {
                    throw new PackageProcessingException("Incorrect XML element algorithm in node security.");
                }
                string exe_path = Path.Combine(pi.ExtractionPath, si.FileName);                
                if (p==null || p.DigestList.Count<NodeCount+1)
                {
                    si.ComputedDigest = s.ComputeMessageDigest(exe_path);
                    if(p!=null)
                        p.DigestList.Add(si.ComputedDigest);
                }
                if(p!=null)
                    si.ComputedDigest = p.DigestList[NodeCount];
                pi.Security.Add(si);
                NodeCount++;
            }
        }
        
        public bool MapPackageName(PackageInfo pi,MANUFACTURER m)
        {
            if (m == MANUFACTURER.SIX)
                return true;
            string manu = Converter.Manu(m);
            string abr=Manufacturer.ManuAbrMapping[manu];
            if (pi.Vendor == abr)
                return true;
            else
                return false;

        }

        public bool ValidateFileName(string fn, PackageInfo pi)
        {
            
            string f=Path.GetFileNameWithoutExtension(fn);
            pi.FileName = f;
            string[] fn_name_parts = f.Split(fn_seperator, StringSplitOptions.RemoveEmptyEntries);
            if (fn_name_parts.Length != 4)
                return false;
            string manu = fn_name_parts[0];
            if (Manufacturer.manu_abr.Contains(manu) == false)
                return false;
            pi.Vendor = manu;
            pi.Name = fn_name_parts[1];
            string v= fn_name_parts[2];
            if (v.Split(version_seperator, StringSplitOptions.RemoveEmptyEntries).Count() != 4)
                return false;
            pi.Version= fn_name_parts[2];
            pi.Date= fn_name_parts[3];
            return true;
        }
        public async Task<bool> ExtractionAsync(SIGNER s,STORETYPE st,ENVIROMENT e,PackageInfo pi,string zip_file)
        {
            try
            {
                
                string m = Converter.Vendor(pi.Vendor);
                string vers = pi.Version;
                string pn = pi.Name;
                string p_name_path = GetVersionExtractionPath(m, s, st,e, vers, pn);
                try
                {
                    Directory.Delete(p_name_path, true);
                    Directory.CreateDirectory(p_name_path);
                }
                catch { }
                await UnzipAsync(p_name_path, zip_file);
                pi.ExtractionPath = p_name_path;
                pi.Executables = new List<string>();
                              
                foreach (string entry in Directory.EnumerateFiles(pi.ExtractionPath))
                {
                    string ext = Path.GetExtension(entry);
                    if (ext == ".info" || ext == ".export" || ext == ".sign")
                    {
                        string f = Path.GetFileNameWithoutExtension(entry);
                        string[] fn_name_parts = f.Split(fn_seperator, StringSplitOptions.RemoveEmptyEntries);
                        if (fn_name_parts.Length != 4)
                        {
                            Error = "Format of Package file name is wrong";
                            return false;
                        }
                        string manu = fn_name_parts[0];
                        if (Converter.Vendor(manu) != m)
                        {
                            Error = "MANU in setup info corresponds not with MANU in file name";
                            return false;
                        }
                        string v = fn_name_parts[2];
                        if (v != pi.Version)
                        {
                            Error = "VERSION in setup info corresponds not with VERSION in file name.";
                            return false;
                        }
                    }
                    else
                        pi.Executables.Add(entry);
                }                
                return true;
            }
            catch
            {
                Error = "General processing error";
                return false;
            }
        }
        private void Backup(string extraction_path)
        {
            if (Directory.EnumerateFiles(extraction_path).Count() == 0)
                return;
            foreach(string fn in Directory.EnumerateFiles(extraction_path))
            {
                string f = Path.GetFileNameWithoutExtension(fn);
                string fileName = Path.GetFileName(fn);                
                string[] fn_name_parts = f.Split(fn_seperator, StringSplitOptions.RemoveEmptyEntries);
                string version = fn_name_parts[2];
                string vers_path = Path.Combine(extraction_path, version);
                if (Directory.Exists(vers_path) == false)
                {
                    Directory.CreateDirectory(vers_path);
                }
                else
                {
                    Directory.Delete(vers_path, true);
                    Directory.CreateDirectory(vers_path);
                }
                Directory.Move(fn, Path.Combine(vers_path, fileName));
            }
        }
        private void Unzip(string target_dir,string zip_file)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zip_file))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(target_dir, entry.FullName));
                    
                }
            }

        }
        private async Task UnzipAsync(string target_dir, string zip_file)
        {
            await Task.Run(async () =>
            {
                string fn=Path.GetFileName(zip_file);
                string localFile= Path.Combine(target_dir, fn);
                await CopyFileAsync(zip_file, localFile);
                zip_file = localFile;
                using (ZipArchive archive = ZipFile.OpenRead(zip_file))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(Path.Combine(target_dir, entry.FullName));
                    }
                }
                File.Delete(zip_file);
            });
        }
        public static async Task CopyFileAsync(string sourceFilePath, string destinationFilePath)
        {
            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: (100*4096), useAsync: true))
            using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: (100 * 4096), useAsync: true))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }

    }
}
