using Infrastructure;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace NavigationModule.Models
{
    public class VersionModel : BindableBase
    {
        string v;
        public string Version
        {
            get { return v; }
            set
            {
                SetProperty(ref v, value);
            }
        }
        ObservableCollection<PackageModel> package_name_list = new ObservableCollection<PackageModel>();
        public ObservableCollection<PackageModel> PackageNameList
        {
            get { return package_name_list; }
            set
            {
                SetProperty(ref package_name_list, value);
            }
        }
        public List<SecurityInfo> Security { get; set; } = new List<SecurityInfo>();

    }
}
