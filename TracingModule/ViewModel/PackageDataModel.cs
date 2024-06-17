using Prism.Mvvm;

namespace TracingModule.ViewModel
{
    public class PackageDataModel : BindableBase
    {
        string error_msg;
        public string ErrorMessage
        {
            get
            {
                return error_msg;
            }
            set
            {
                SetProperty(ref error_msg, value);
            }
        }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetProperty(ref name, value);
            }
        }
        string version;
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                SetProperty(ref version, value);
            }
        }
        string date;
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                SetProperty(ref date, value);
            }
        }
        string vendor;
        public string Vendor
        {
            get
            {
                return vendor;
            }
            set
            {
                SetProperty(ref vendor, value);
            }
        }
        //public void SetData(TracePackageData pd,string ErrorMsg)
        //{
        //    Name = pd.PackageName;
        //    Version = pd.Version;
        //    Date = pd.Date;
        //    Vendor = pd.Vendor;
        //    ErrorMessage = ErrorMsg;
        //}
        public void ClearData()
        {
            Name = string.Empty;
            Version = string.Empty;
            Date = string.Empty;
            Vendor = string.Empty;
        }

    }
}
