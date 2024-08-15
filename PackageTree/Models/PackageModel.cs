using Prism.Mvvm;

namespace NavigationModule.Models
{
    public class PackageModel : BindableBase
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
        string m;
        public string Manu
        {
            get { return m; }
            set
            {
                SetProperty(ref m, value);
            }
        }
        string pn;
        public string PackageName
        {
            get { return pn; }
            set
            {
                SetProperty(ref pn, value);
            }
        }
        bool is_selected;
        public bool IsSelected
        {
            get { return is_selected; }
            set
            {
                SetProperty(ref is_selected, value);
            }
        }
    }
}
