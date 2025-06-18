using Infrastructure;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareSigning.Model
{
    public class PackageInfoModel : BindableBase
    {
        public PackageInfoModel()
        {

        }
        public PackageInfo GetPackageInfo()
        {
            PackageInfo pi = new PackageInfo();            
            pi.Vendor=Vendor;
            pi.PackageProvider = PackageProvider;
            pi.Version=Version;
            pi.Name=Name;
            pi.ExtractionPath=ExtractionPath;
            pi.Executables=Executables;
            pi.Date=PackageDate;
            pi.FileName = FileName;
            if(Security!=null)
                pi.Security = new List<SecurityInfo>(Security);
            return pi;
        }
        public PackageInfoModel(PackageInfo pi)
        {
            Vendor = pi.Vendor;
            PackageProvider = Converter.Vendor(pi.Vendor);
            Version = pi.Version;
            Name = pi.Name;
            ExtractionPath = pi.ExtractionPath;
            Executables = pi.Executables;
            if (Executables != null)
                NuOfExecutables = pi.Executables.Count();
            else
                NuOfExecutables = 0;
            PackageDate = pi.Date;
            FileName = pi.FileName;
            if(pi.Security!=null)
                Security = new List<SecurityInfo>(pi.Security);
        }
        string fn;
        public string FileName
        {
            get { return fn; }
            set
            {
                SetProperty(ref fn, value);
            }
        }
        string vendor;
        public string Vendor
        {
            get { return vendor; }
            set
            {
                SetProperty(ref vendor, value);
            }
        }
        string packageprovider;
        public string PackageProvider
        {
            get { return packageprovider; }
            set
            {
                SetProperty(ref packageprovider, value);
            }
        }
        string version;
        public string Version
        {
            get { return version; }
            set
            {
                SetProperty(ref version, value);
            }
        }
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }
        string extraction_path;
        public string ExtractionPath
        {
            get { return extraction_path; }
            set
            {
                SetProperty(ref extraction_path, value);
            }
        }
        string package_date;
        public string PackageDate
        {
            get { return package_date; }
            set
            {
                SetProperty(ref package_date, value);
            }
        }
        string _dropStatus;
        public string DropStatus
        {
            get { return _dropStatus; }
            set
            {
                SetProperty(ref _dropStatus, value);
            }
        }
        
        public List<string> Executables { get; set; }
        public List<SecurityInfo> Security { get; set; }

        int nu_executable;
        public int NuOfExecutables
        {
            get { return nu_executable; }
            set
            {
                SetProperty(ref nu_executable, value);
            }
        }
       

    }
}
