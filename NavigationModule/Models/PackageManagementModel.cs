using Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unity;

namespace NavigationModule.Models
{
    public class PackageManagementModel :  BindableBase
    {
        IUnityContainer _container;
        
        public ICommand SelectedItemChangedCommand { get; private set; }
        private PackageModel seletectedItem;
        Action<PackageModel> ManuSoftwareSigning;

        public PackageManagementModel(IUnityContainer container, Action<PackageModel> _ManuSoftwareSigning, string[] manufacturers)
        {
            _container = container;
            ManuSoftwareSigning = _ManuSoftwareSigning;
            
            SelectedItemChangedCommand = new DelegateCommand<object>(OnItemChanged);
           
            foreach (string manu in manufacturers)
            {
                ManufacturerPackagesModel m = new ManufacturerPackagesModel();
                m.Manufacturer = manu;
                ManuPackages.Add(m);
            }
        }
        private void OnItemChanged(object o)
        {
            if (o is PackageModel)
            {
                seletectedItem = (PackageModel)o;
                ManuSoftwareSigning(seletectedItem);
            }
            else
                seletectedItem = null;
            object d = o;
        }
        
        ObservableCollection<ManufacturerPackagesModel> manu_packages = new ObservableCollection<ManufacturerPackagesModel>();
        public ObservableCollection<ManufacturerPackagesModel> ManuPackages
        {
            get
            {
                return manu_packages;
            }
            set
            {
                SetProperty(ref manu_packages, value);               
            }
        }
        ObservableCollection<VersionModel> single_manu_packages=new ObservableCollection<VersionModel>();
        public ObservableCollection<VersionModel> SingleManuPackages
        {
            get
            {
                return single_manu_packages;
            }
            set
            {
                SetProperty(ref single_manu_packages, value);
            }
        }
        public void SetManuPackages(MANUFACTURER m)
        {
            SingleManuPackages.Clear();
            string manu = Converter.Manu(m);
            try
            {
                var r = ManuPackages.First(e => e.Manufacturer == manu);           
                foreach (var e in r.VersionList)
                {
                    SingleManuPackages.Add(e);
                }
            }
            catch { }
        }
        
        public void LoadAllPackages(string signer,STORETYPE st,ENVIROMENT e)
        {
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIGNER s = Converter.Signer(signer);
            
            foreach (ManufacturerPackagesModel mp in ManuPackages)
            {
                mp.VersionList.Clear();               
                string ep = pp.GetExtractionPath(mp.Manufacturer,s,st,e);
                List<string> versions = pp.ExtractedVersions(ep);                
                foreach (string ver in versions)
                {
                    VersionModel v = new VersionModel();
                    v.Version = ver;
                    List<string> package_names = pp.GetPackageNameList(ep, ver);
                    foreach(string pn in package_names)
                    {
                        PackageModel pm = new PackageModel();
                        pm.Manu = mp.Manufacturer;
                        pm.Version = v.Version;
                        pm.PackageName = pn;
                        v.PackageNameList.Add(pm);
                    }
                    mp.VersionList.Add(v);
                }
                
            }
        }
        
    }

    
    
    
    
}
