using Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unity;
using NavigationModule;

namespace NavigationModule.Models
{
    public class PackageManagementModel :  BindableBase
    {
        IUnityContainer _container;
        
        public ICommand SelectedItemChangedCommand { get; private set; }
        private PackageModel seletectedItem;
        public Action<PackageModel> ManuSoftwareSigning { get; set; }
        public void SetManufacturers(string[] manufacturers)
        {
            foreach (string manu in manufacturers)
            {
                ManufacturerPackagesModel m = new ManufacturerPackagesModel();
                m.Manufacturer = manu;
                ManuPackages.Add(m);
            }
        }

        public PackageManagementModel(IUnityContainer container)
        {
            _container = container;           
            SelectedItemChangedCommand = new DelegateCommand<object>(OnItemChanged);
           
            
        }
        private void OnItemChanged(object o)
        {
            if (o is PackageModel)
            {
                SelectedTreeViewItem = o;
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
        object _selectedItem;
        public object SelectedTreeViewItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                SetProperty(ref _selectedItem, value);
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
        public ManufacturerPackagesModel LoadPackage(string manu,string signer,STORETYPE st,ENVIROMENT e,string version)
        {
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIGNER s = Converter.Signer(signer);
            ManufacturerPackagesModel mp = ManuPackages.First(e => e.Manufacturer == manu);
            mp.VersionList.Clear();
            string ep = pp.GetExtractionPath(mp.Manufacturer, s, st, e);
            List<string> versions = pp.ExtractedVersions(ep);
            string v=versions.FirstOrDefault(e => e == version);
            PackageModel pm = new PackageModel();
            if (!string.IsNullOrEmpty(v))
            {
                VersionModel vm = new VersionModel();
                vm.Version = v;
                List<string> package_names = pp.GetPackageNameList(ep, v);
                foreach (string pn in package_names)
                {
                    
                    pm.Manu = mp.Manufacturer;
                    pm.Version = vm.Version;
                    pm.PackageName = pn;
                    vm.PackageNameList.Add(pm);
                }
                mp.VersionList.Add(vm);
            }  
            return mp;
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
