using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace NavigationModule.Models
{
    public class ManufacturerPackagesModel : BindableBase
    {
        string manu_name;
        public string Manufacturer
        {
            get
            {
                return manu_name;
            }
            set
            {
                SetProperty(ref manu_name, value);
            }
        }
        ObservableCollection<VersionModel> versions = new ObservableCollection<VersionModel>();
        public ObservableCollection<VersionModel> VersionList
        {
            get
            {
                return versions;
            }
            set
            {
                SetProperty(ref versions, value);
            }
        }
    }
}
