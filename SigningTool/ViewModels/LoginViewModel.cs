using Infrastructure;

using Unity;
using NavigationModule;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SigningTool.Views;
using SoftwareSigning.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Infrastructure.Pkcs11wrapper;

namespace SigningTool.ViewModels
{
    public class LoginViewModel : BindableBase, INavigationAware
    {
        
        private string[] manu = Manufacturer.manu;
        private string[] qa_atm = { "TEST-UNMANAGED", "TEST-KMS", "PROD-KMS" };
        private string[] atm_device = { "TEST-UNMANAGED","TEST-KMS", "PROD-KMS" };
        bool modifiedHSMAdr = false;
        
        IRegionManager _manager;
        IUnityContainer _container;
        NavigationParameters parameters;
        LoginStatusBarViewModel _status;
        //StorePasswordSafe PwdManagement;
        Uri navigationURI;
        public ICommand LoginCommand { get; private set; }
        public DelegateCommand<object> EnterKeyCommand { get; }
        public DelegateCommand ModifyConnectionCommand { get; private set; }

        public ICommand SelectedCommandQA { get; private set; }
        public ICommand SelectedCommandATM { get; private set; }
        public ICommand SelectedCommandManuDev { get; private set; }
        public ICommand SelectedCommandManuProd { get; private set; }
        public ICommand SelectedCommandATMDevice { get; private set; }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        bool _isOperationInProgress;
        public bool IsOperationInProgress
        {
            get { return _isOperationInProgress; }
            set
            {
                SetProperty(ref _isOperationInProgress, value);
            }
        }
        public MANUFACTURER Manu { get; set; }
        LoginView view;
        public LoginViewModel(IRegionManager manager,IUnityContainer container)
        {
            _manager = manager;
            _container = container;
            ConfigurationAccess cfg = new ConfigurationAccess();
            _status = _container.Resolve<LoginStatusBarViewModel>();
            _status.Error("APPLICATION START", "HSM initialzation still in progress");
            IsOperationInProgress=true;
            HSMConnectionText ="Connection to HSM still in progress";    
            HSMIP = cfg.AccessKey("CK_HSM_IP_ADR");                      
            try
            {
                LastName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                LastName = "";
            }            
            StorePassword = "1234";
            
            ModifyConnectionCommand=new DelegateCommand(this.OnModifyConnection);            
            EnterKeyCommand = new DelegateCommand<object>(OnEnterKeyPressed);
            this.LoginCommand = new DelegateCommand(this.OnLogin);
            this.SelectedCommandQA = new DelegateCommand(this.SetQAEnviroment);
            this.SelectedCommandATM = new DelegateCommand(this.SetATMEnviroment);
            this.SelectedCommandManuDev = new DelegateCommand(this.SetMANUDevEnviroment);
            this.SelectedCommandManuProd = new DelegateCommand(this.SetMANUProdEnviroment);
            this.SelectedCommandATMDevice = new DelegateCommand(this.SetATMDeviceEnviroment);
            SIATM = -1;
            SIManuDev = -1;
            SIManuProd = -1;
            SIQA = -1;
            SIATM_DEVICE = -1;
            SelectedEnviroment = string.Empty;
            
            session_date = DateTime.Now.ToShortDateString();
            session_id = new Guid().ToString();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string Vers= "V "+version.Major.ToString()+".";
            Vers+=version.Minor.ToString();
            Vers+=" BUILD "+version.Build.ToString();
            Version = Vers;
            manu_list = new ObservableCollection<string>(manu);
            InitializeHSMAsync();
        }
        private async void InitializeHSMAsync()
        {
            try
            {
                // Perform asynchronous operations here
                await HSMInitializationAsyncOperation();
                _status.Success("APPLICATION START", "HSM initialization successfull");
                HSMConnectionText = "HSM Connection established to: "+HSMIP;
                IsOperationInProgress = false;
            }
            catch 
            {
                // Handle exceptions
                _status.Error("HSM INITIALIZATION", "HSM initialization failed. Please modify IP address for HSM");
                HSMConnectionText = "HSM Connection failed to: " + HSMIP;
                IsOperationInProgress= false;
            }
        }

        private async Task HSMInitializationAsyncOperation()
        {
            await Task.Run(() => 
            {
                var driver = PKCS11Driver.GetInstance();
            });
        }
        private void OnModifyConnection()
        {
            if(string.IsNullOrEmpty(HSMIP) == true)
            {
                HSMConnectionText = "HSM IP not modified";
                return;
            }
            ConfigurationAccess cfg = new ConfigurationAccess();
            cfg.AddKey("CK_HSM_IP_ADR", HSMIP);            
            cfg.SetRegistry(@"SOFTWARE\SafeNet\HSM\NETCLIENT", "ET_HSM_NETCLIENT_SERVERLIST", HSMIP);
            HSMConnectionText = "HSM IP modified to: " + HSMIP ;
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            status.Error("HSM IP MODIFIED","Application has to be restarted to activate new HSM IP address");
            modifiedHSMAdr = true;
        }
        private void OnEnterKeyPressed(object parameter)
        {
            if (parameter is KeyEventArgs keyEventArgs)
            {
                // Retrieve the key that was pressed
                Key pressedKey = keyEventArgs.Key;

                // Perform actions based on the pressed key
                if (pressedKey == Key.Enter)
                {
                    StorePassword = Password;
                    OnLogin();
                }
               
            }
        }
        private void SetATMDeviceEnviroment()
        {
            
            if (SIATM_DEVICE < 0)
                return;
            SelectedEnviroment = "SIX-ATM-DEVICE-" + atm_device[SIATM_DEVICE];
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();            
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            parameters = new NavigationParameters();
            parameters.Add("MANU", "SIX-ATM");
            navigationURI = NavigationURI.NavigationSIXQAViewUri;
            Manu = MANUFACTURER.SIX;
            _signing.Origin = "SIX-ATM-DEVICE";
            _signing.Signer = "SIX";
            _signing.SignerType = "ATM-DEVICE";
            
            switch (SIATM_DEVICE)
            {
                case 0:
                    _signing.StoreType = "UNMANAGED";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    status.Success("ATM DEVICE ENVIROMENT SELECTED", "Default Password for KEY STORE used.");
                    StorePassword = PwdManagement.DefaultStorePwd;
                    break;
                case 1:
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    status.Success("ATM DEVICE ENVIROMENT SELECTED", "Enter Password for HSM access");
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.ATM);
                    break;
                case 2:
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "PROD";
                    parameters.Add("ENV", "PROD");
                    status.Success("ATM DEVICE ENVIROMENT SELECTED", "Enter Password for HSM access");
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.PROD, CERTTYPE.ATM);
                    break;
            }
            SIATM = -1;
            SIManuDev = -1;
            SIManuProd = -1;
            SIQA = -1;
        }
        private void SetQAEnviroment()
        {
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            
            if (SIQA < 0)
                return;
            //if (SIQA >= qa_atm.Length)
            //    return;

            Manu = MANUFACTURER.SIX;
            //SelectedEnviroment = "SIX-QA-" + qa_atm[SIQA];
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            parameters =new NavigationParameters();
            parameters.Add("MANU", "SIX-QA");

            
           
            switch (SIQA)
            {
                
                case 0:
                    _signing.Origin = "SIX-QA";
                    _signing.Signer = "SIX";
                    _signing.SignerType = "QA";
                    SelectedEnviroment = "SIX-QA";                    
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "TEST";                    
                    parameters.Add("ENV", "TEST");
                    navigationURI = NavigationURI.NavigationSIXQAViewUri;//NavigationURI.NavigationSIXManagedViewUri;
                    PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.QA);
                    status.Success("KMS ENVIROMENT SELECTED", "Enter Password for HSM access");                    
                    StorePassword=PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.QA);
                    break;
                case 1:
                    _signing.Origin = "SIX-QA";
                    _signing.Signer = "SIX";
                    _signing.SignerType = "QA";
                    SelectedEnviroment = "SIX-QA";
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    navigationURI = new Uri("NavigationSIXManagedView", UriKind.Relative);
                    PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.QA);
                    status.Success("KMS ENVIROMENT SELECTED", "Enter Password for HSM access");
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.QA);
                    break;
                case 2:
                    _signing.Signer = "SIX";
                    _signing.Origin = "SIX-ATM";
                    _signing.SignerType = "ATM";
                    SelectedEnviroment = "SIX-ATM-TEST";
                    navigationURI = new Uri("NavigationSIXManagedView", UriKind.Relative);//NavigationURI.NavigationSIXManagedViewUri;
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    status.Success("KMS ENVIROMENT SELECTED", "Enter Password for HSM access");
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.ATM);
                    break;
                case 3:
                    _signing.Signer = "SIX";
                    _signing.Origin = "SIX-ATM";
                    _signing.SignerType = "ATM";
                    SelectedEnviroment = "SIX-ATM-PROD";
                    navigationURI = new Uri("NavigationSIXManagedView", UriKind.Relative);//NavigationURI.NavigationSIXManagedViewUri;
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "PROD";
                    parameters.Add("ENV", "PROD");
                    status.Success("KMS ENVIROMENT SELECTED", "Enter Password for HSM access");
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.PROD, CERTTYPE.ATM);
                    break;

            }
            SIATM = -1;
            SIManuDev = -1;
            SIManuProd = -1;
            SIATM_DEVICE = -1;
        }
        
        private void SetATMEnviroment()
        {
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            if (SIATM < 0)
                return;
            //if (SIATM >= qa_atm.Length)
            //    return;
            Manu = MANUFACTURER.SIX;
            SelectedEnviroment = "SIX-ATM-" + qa_atm[SIATM];
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            parameters = new NavigationParameters();
            parameters.Add("MANU", "SIX-ATM");

            navigationURI = NavigationURI.NavigationSIXQAViewUri;
            _signing.Signer = "SIX";
            _signing.Origin = "SIX-ATM";
            _signing.SignerType = "ATM";
            
            switch (SIATM)
            {
                case 0:
                    navigationURI = NavigationURI.NavigationSIXQAViewUri;
                    _signing.Signer = "SIX";
                    _signing.Origin = "SIX-ATM";
                    _signing.SignerType = "ATM";
                    _signing.StoreType = "UNMANAGED";
                    _signing.Enviroment = "TEST";                    
                    parameters.Add("ENV", "TEST");
                    status.Success("UNMANAGED ENVIROMENT SELECTED", "Default Password for KEY STORE used.");
                    StorePassword = PwdManagement.DefaultStorePwd;
                    break;
                case 1:
                    _signing.Origin = "SIX-QA";
                    _signing.Signer = "SIX";
                    _signing.SignerType = "QA";
                    SelectedEnviroment = "SIX-QA-" + "UNMANAGED";//qa_atm[SIQA];
                    _signing.StoreType = "UNMANAGED";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    navigationURI = NavigationURI.NavigationSIXQAViewUri;
                    status.Success("UNMANAGED ENVIROMENT SELECTED", "Default Password for KEY STORE used.");
                    StorePassword = PwdManagement.DefaultStorePwd;
                    break;
                case 2:
                    navigationURI = NavigationURI.NavigationSIXManagedViewUri;
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "PROD";                    
                    parameters.Add("ENV", "PROD");
                    status.Success("KMS ENVIROMENT SELECTED", "Enter Password for HSM access");                    
                    StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.PROD, CERTTYPE.ATM);
                    break;
            }
            SIQA = -1;
            SIManuDev = -1;
            SIManuProd = -1;
            SIATM_DEVICE = -1;
        }
        private void SetMANUDevEnviroment()
        {
                        
            if (SIManuDev < 0)
                return;
            if (SIManuDev >= manu.Length)
                return;
            

            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            MANUFACTURER m = Manu=Converter.Manu(manu[SIManuDev]);
            StorePassword = PwdManagement.GetStorePassword(m, ENVIROMENT.TEST);
            if (PwdManagement.ManuStoreExists(m,ENVIROMENT.TEST)==true)
                status.Success("MANUFACTUER TEST ENVIROMENT SELECTED", "Enter Password for KEY STORE access");
            else
                status.Success("MANUFACTUER TEST ENVIROMENT SELECTED", "No KEY STORE available. Enter Password for new KEY STORE");
            SelectedEnviroment = "MANU-" + manu[SIManuDev];
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            _signing.Origin = "MANU-"+ manu[SIManuDev];
            _signing.Signer = manu[SIManuDev];            
            _signing.SignerType = "MANU";
            _signing.StoreType = "UNMANAGED";
            _signing.Enviroment = Converter.Env(ENVIROMENT.TEST);
            parameters = new NavigationParameters();
            parameters.Add("MANU", _signing.Origin);
            parameters.Add("ENV", "TEST");
            navigationURI = NavigationURI.NavigationMANUViewUri;

            SIQA = -1;
            SIATM = -1;
            SIManuProd = -1;
            SIATM_DEVICE = -1;
        }
        private void SetMANUProdEnviroment()
        {
            ConfigurationAccess cfg = new ConfigurationAccess();
            HSMIP = cfg.ReadRegistry(@"SOFTWARE\SafeNet\HSM\NETCLIENT", "ET_HSM_NETCLIENT_SERVERLIST");

            if (SIManuProd < 0)
                return;
            if (SIManuProd >= manu.Length)
                return;
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            MANUFACTURER m = Manu=Converter.Manu(manu[SIManuProd]);
            StorePassword = PwdManagement.GetStorePassword(m, ENVIROMENT.PROD);
            if (PwdManagement.ManuStoreExists(m, ENVIROMENT.PROD) == true)
                status.Success("MANUFACTUER PROD ENVIROMENT SELECTED", "Enter Password for PROD KEY STORE access");
            else
                status.Success("MANUFACTUER PROD ENVIROMENT SELECTED", "No KEY STORE available. Enter Password for new KEY STORE");
            SelectedEnviroment = "MANU-" + manu[SIManuProd];
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            _signing.Origin = "MANU-" + manu[SIManuProd];
            _signing.Signer = manu[SIManuProd];
            _signing.SignerType = "MANU";
            _signing.StoreType = "UNMANAGED";
            _signing.Enviroment = Converter.Env(ENVIROMENT.PROD);
            parameters = new NavigationParameters();
            parameters.Add("MANU", _signing.Origin);
            parameters.Add("ENV", "PROD");
            navigationURI = NavigationURI.NavigationMANUViewUri;

            SIQA = -1;
            SIATM = -1;
            SIManuDev = -1;
            SIATM_DEVICE = -1;
        }
        private void OnLogin()
        {                     
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();            
            if (CheckInput(_signing) == false)
                return;
            //Log(_signing);           
            _signing.FirstName = FirstName;
            _signing.LastName = LastName;
            _signing.SessionDate = SessionDate;
            _manager.RequestNavigate("NavigationRegion", navigationURI, parameters);         
        }
        
        private bool  CheckInput(SIXSoftwareSigningViewModel _signing)
        {
            LoginStatusBarViewModel status = _container.Resolve<LoginStatusBarViewModel>();
            if(modifiedHSMAdr==true)
            {
                status.Error("HSM IP ADDRESS", "HSM IP address modified. Please restart application.");
                return false;
            }
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            if (SIQA == -1 && SIATM == -1 && SIManuDev == -1 && SIManuProd == -1 && SIATM_DEVICE == -1)
            {
                status.Error("SESSION START", "No entity selected");
                return false;
            }           
            try
            {
                Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
            }
            catch(Exception eX)
            {
                status.Error("HSM CONNECTION", "NO TOKEN CONNECTED: " + eX);
                return false;
            }
            
            string e = (string)parameters["ENV"];
            if (_signing.StoreType == "UNMANAGED" && _signing.SignerType == "MANU")
            {
                if (PwdManagement.ManuStoreExists(Converter.Manu(_signing.Signer), Converter.Env(_signing.Enviroment)) == true)
                {
                    if (PwdManagement.PasswordChanged == true)
                    {
                        StorePassword = PwdManagement.GetStorePassword(Converter.Manu(_signing.Signer), Converter.Env(_signing.Enviroment));
                        PwdManagement.PasswordChanged = false;
                    }
                    if (PwdManagement.CheckManuStorePwd(Converter.Manu(_signing.Signer), Converter.Env(_signing.Enviroment), StorePassword) == false)
                    {
                        status.Error("SESSION START", "Wrong password for KEY STORE access.");
                        return false;
                    }
                    else
                        PwdManagement.SetStorePassword(Converter.Manu(_signing.Signer), Converter.Env(_signing.Enviroment), StorePassword);
                }
                else
                {                    
                    PwdManagement.SetStorePassword(Converter.Manu(_signing.Signer), Converter.Env(_signing.Enviroment), StorePassword);
                }

            }
            if (_signing.StoreType == "KMS")
            {
                if (string.IsNullOrEmpty(StorePassword) == true)
                {
                    status.Error("SESSION START", "Missing password for HSM access.");
                    return false;
                }
                
                if (PwdManagement.CheckHSMPwd(StorePassword, e)==false)
                {
                    status.Error("HSM PASSWORD CHECK", "Wrong password. Please reenter correct password.");
                    return false;
                }
                
                PwdManagement.AddSIXPwd(STORETYPE.KMS, Converter.Env(e),Converter.CertType(_signing.SignerType), StorePassword);
                PwdManagement.AddSIXPwd(STORETYPE.KMS, ENVIROMENT.PROD, CERTTYPE.ATM, StorePassword);
                PwdManagement.AddSIXPwd(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.ATM, StorePassword);
                PwdManagement.AddSIXPwd(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.QA, StorePassword);
            }
            return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ConfigurationAccess cfg = new ConfigurationAccess();
            HSMIP = cfg.ReadRegistry(@"SOFTWARE\SafeNet\HSM\NETCLIENT", "ET_HSM_NETCLIENT_SERVERLIST");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            return;
        }

        ObservableCollection<string> manu_list;
        public ObservableCollection<string> ManuList
        {
            get
            {
                return manu_list;
            }
            set
            {
                SetProperty(ref manu_list, value);
            }
        }
        string hsmConnectionText;
        public string HSMConnectionText
        {
            get { return hsmConnectionText; }
            set
            {
                SetProperty(ref hsmConnectionText, value);
            }
        }
        string hsmip;
        public string HSMIP
        {
            get { return hsmip; }
            set
            {
                SetProperty(ref hsmip, value);
            }
        }
        string store_pwd;
        public string StorePassword
        {
            get { return store_pwd; }
            set
            {
                SetProperty(ref store_pwd, value);                
            }
        }
        string soft_version;
        public string Version
        {
            get { return soft_version; }
            set
            {
                SetProperty(ref soft_version, value);
            }
        }
        int si_manu_dev;
        public int SIManuDev
        {
            get { return si_manu_dev; }
            set
            {
                SetProperty(ref si_manu_dev, value);
            }
        }
        int si_manu_prod;
        public int SIManuProd
        {
            get { return si_manu_prod; }
            set
            {
                SetProperty(ref si_manu_prod, value);
            }
        }
        
        int si_atm_device;
        public int SIATM_DEVICE
        {
            get { return si_atm_device; }
            set
            {
                SetProperty(ref si_atm_device, value);
            }
        }
        int si_qa;
        public int SIQA
        {
            get { return si_qa; }
            set
            {
                SetProperty(ref si_qa, value);
            }
        }
        int si_atm;
        public int SIATM
        {
            get { return si_atm; }
            set
            {
                SetProperty(ref si_atm, value);
            }
        }
        string selected_env;
        public string SelectedEnviroment
        {
            get { return selected_env; }
            set
            {
                SetProperty(ref selected_env, value);

            }
        }
        string first_name;
        string last_name;
        public string FirstName
        {
            get { return first_name; }
            set
            {
                SetProperty(ref first_name, value);
                
            }
        }
        public string LastName
        {
            get { return last_name; }
            set
            {
                SetProperty(ref last_name, value);
                
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
        string session_id;
        public string SessionID
        {
            get
            {
                return session_id;
            }
            set
            {
                SetProperty(ref session_id, value);
            }
        }
    }
}
