using Microsoft.Extensions.Logging;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

namespace Infrastructure.Pkcs11wrapper
{

    public class PKCS11Driver : IPKCS11Driver
    {
        private static PKCS11Driver _instance=null;
        ILogger<PKCS11Driver> _logger;
        IPkcs11Library _pkcs11 = null;
        const string _defaultTokenName = "ATM_SOFTWARE_SIGNING";
        string _tokenName = _defaultTokenName;

        ISlot _slot = null;
        bool initializationInProgress = false;
        public bool Initialized { get; private set; }
        public bool AbortInitialization { get; set; }
        public ISlot GetSlot
        {
            get { return _slot; }
        }
        public string TokenName
        {
            get { return _tokenName; }
            set { _tokenName = value; }
        }

        public string DefaultTokenName
        {
            get { return _defaultTokenName; }
        }
        public static IPKCS11Driver GetDisposableInstance() 
        {
            return _instance;
        }
        public static IPKCS11Driver GetInstance()
        {
            if (_instance == null)
                _instance = new PKCS11Driver(null);            
            return _instance;
        }
        public static IPKCS11Driver GetInstance(ILogger<PKCS11Driver> logger) 
        {
            if (_instance == null)
            {                
                _instance = new PKCS11Driver(logger);
            }
            else
                _instance._logger = logger;
            return _instance;
        }

        private PKCS11Driver(ILogger<PKCS11Driver> logger) 
        {
            _logger = logger;
            Initialized = false;
            Initialize();
            InitSlots();
        }
        private void Initialize()
        {
            if (_pkcs11 != null)
                _pkcs11.Dispose();

            try
                {
                    Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
                    _pkcs11 = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, @"cryptoki", AppType.MultiThreaded);
                    Initialized = true;
                    if (_logger != null)
                        _logger.LogDebug("HSM driver successfull initialized");
                }
                catch(Pkcs11Exception e) 
                {                    
                    if (_logger != null)
                        _logger.LogCritical(e, HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.LOAD_DRIVER));
                    _pkcs11 = null;
                    Initialized = false;
                    throw HSMDriverExceptionFactory.Create((int)e.RV, HSMDriverErrorDefinitions.LOAD_DRIVER);                    
                }
                catch (Exception e)
                {
                    if (_logger != null)
                        _logger.LogCritical(e, HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.LOAD_DRIVER));
                    _pkcs11 = null;
                    Initialized = false;
                    throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.LOAD_DRIVER);                    
                }                                
                return;
            
            

        }
        public void Renew()
        {
            Initialize();
        }

        public void UnInitialize()
        {
            if (_logger != null)
                _logger.LogDebug("UnInitialize HSM driver cryptoki.dll");
            AbortInitialization = true;            
        }
        private void InitSlots()
        {
            foreach (ISlot slot in _pkcs11.GetSlotList(SlotsType.WithTokenPresent))
            {
                ISlotInfo slotInfo = slot.GetSlotInfo();
                ITokenInfo info = slot.GetTokenInfo();
                string label = info.Label;
                if (label == _tokenName)
                {
                    _slot = slot;
                    Initialized = true;
                    break;
                }
            }
            if (_slot == null)
            {
                Initialized = false;
                if (_logger != null)
                    _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.SELECT_SLOT)+": " +_tokenName);
                throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.SELECT_SLOT, _tokenName);                
            }
           

        }
        
       

        public void ChangeToken(string name)
        {
            if (Initialized == false)
                return;
            if (_logger != null)
                _logger.LogInformation("Close all sessions in " + TokenName + " and change token to " + TokenName);
            TokenName = name;            
            try
            {
                _slot.CloseAllSessions();
            }
            catch { }
            InitSlots();
        }
    }
}
