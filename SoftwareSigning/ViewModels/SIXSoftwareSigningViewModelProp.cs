using Prism.Mvvm;
using Prism.Regions;
using SoftwareSigning.Model;
using System.Windows;
using System.Windows.Media;

namespace SoftwareSigning.ViewModels
{
    public partial class SIXSoftwareSigningViewModel: BindableBase, INavigationAware
    {
        SoftwareSigningToolbarViewModel _tvbm;
        public SoftwareSigningToolbarViewModel ToolbarViewModel 
        {
            get { return _tvbm; }
            set { SetProperty(ref _tvbm, value); }
        }
        PackageDropModel _pd;
        public PackageDropModel PackageDrop
        {
            get { return _pd; }
            set
            {
                SetProperty(ref _pd, value);
            }
        }
        Visibility pss_vis;
        public Visibility PSSVisibility
        {
            get
            {
                return pss_vis;
            }
            set
            {
                SetProperty(ref pss_vis, value);
            }
        }

        Brush manu_cert_req_status;
        public Brush ManuCertReqStatus
        {
            get { return manu_cert_req_status; }
            set
            {
                SetProperty(ref manu_cert_req_status, value);
            }
        }
        bool atm_export_enabled;
        public bool ATMExportEnabled
        {
            get { return atm_export_enabled; }
            set
            {
                SetProperty(ref atm_export_enabled, value);
            }
        }
        Brush atm_signing_status;
        public Brush ATMSigningStatus
        {
            get { return atm_signing_status; }
            set
            {
                SetProperty(ref atm_signing_status, value);
            }
        }
        Brush signing_status;
        public Brush SigningStatus
        {
            get { return signing_status; }
            set
            {
                SetProperty(ref signing_status, value);
            }
        }
        Brush sig_status;
        public Brush SignatureStatus
        {
            get { return sig_status; }
            set
            {
                SetProperty(ref sig_status, value);
            }
        }
        Brush cer_val_status;
        public Brush CertifcateValidityStatus
        {
            get { return cer_val_status; }
            set
            {
                SetProperty(ref cer_val_status, value);
            }
        }

        string export_signature_exits;
        public string ExportSignatureExists
        {
            get { return export_signature_exits; }
            set
            {
                SetProperty(ref export_signature_exits, value);
            }
        }
        string sig_cert_owner;
        public string SignerCertificateOwner
        {
            get { return sig_cert_owner; }
            set
            {
                SetProperty(ref sig_cert_owner, value);
            }
        }
        string ver_cert_owner;
        public string VerificationCertificateOwner
        {
            get { return ver_cert_owner; }
            set
            {
                SetProperty(ref ver_cert_owner, value);
            }
        }
        
        string store_type;
        public string StoreType
        {
            get { return store_type; }
            set
            {
                SetProperty(ref store_type, value);
            }
        }
        string package_provider_title;
        public string PackageProviderTitle
        {
            get { return package_provider_title; }
            set
            {
                SetProperty(ref package_provider_title, value);
            }
        }

        PackageInfoModel pi;
        public PackageInfoModel PI
        {
            get { return pi; }
            set
            {
                SetProperty(ref pi, value);
            }
        }
        string firstname;
        public string FirstName
        {
            get { return firstname; }
            set
            {
                SetProperty(ref firstname, value);
            }
        }
        string lastname;
        public string LastName
        {
            get { return lastname; }
            set
            {
                SetProperty(ref lastname, value);
            }
        }
        string origin;
        public string Origin
        {
            get { return origin; }
            set
            {
                SetProperty(ref origin, value);
            }
        }
        string session_date;
        public string SessionDate
        {
            get
            {
                return session_date;
            }
            set
            {
                SetProperty(ref session_date, value);
            }
        }
        string env;
        public string Enviroment
        {
            get
            {
                return env;
            }
            set
            {
                SetProperty(ref env, value);
            }
        }        
        
    }
}
