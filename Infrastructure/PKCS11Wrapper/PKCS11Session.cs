
using Microsoft.Extensions.Logging;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI41;
using System.Text;


namespace Infrastructure.Pkcs11wrapper;

public class PKCS11Session : IPKCS11Session
{
    //Vendor specific  mechanism not part of pkcs11 intop lib.        
    IPKCS11Driver _library;        
    ISession? _session;    
    public ISession HSMSession
    {
        get { return _session; }
    }
    ILogger<PKCS11Session> _logger;   
    DateTime _sessionCreation;
    public DateTime SessionCreation
    {
        get
        {
            return _sessionCreation;
        }
        set 
        {
            _sessionCreation = value;
        }
    }

    public PKCS11Session(ILogger<PKCS11Session> logger,ILogger<PKCS11Driver> driverLogger)
    {            
        _library = PKCS11Driver.GetInstance(driverLogger);
        _logger = logger;        
        _sessionCreation = DateTime.Now;
    }

    public void OpenReadOnlySession(string pwd)
    {            
        OpenSession(pwd, true);
        _sessionCreation = DateTime.Now;
    }
    public void OpenWriteSession(string pwd)
    {
        OpenSession(pwd, false);
        _sessionCreation = DateTime.Now;
    }
    public void CloseSession()
    {
        if (_session == null)
            return;
        try
        {
            _session.CloseSession();
            _session.Dispose();
            _session = null;
        }
        catch
        {
            _session = null;
        }
    }
    public string ReadPublicKey(string label)
    {
        if(_session==null)
        {
            if (_logger != null)
                _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.SESSION_NOT_OPEN));
            throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.SESSION_NOT_OPEN);
        }
        List<IObjectAttribute> searchAttributes = PKCS11AttributeBuilder.PublicKeySearch(label);
        List<IObjectHandle> foundObjects = _session.FindAllObjects(searchAttributes);
        if(foundObjects.Count == 0)
        {
            if (_logger != null)
                _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.MISSING_PUBLIC_KEY)+label);
            throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.MISSING_PUBLIC_KEY);
        }   
        List<IObjectAttribute> key=_session.GetAttributeValue(foundObjects[0], new List<CKA> { CKA.CKA_VALUE });
        var k = key[0].GetValueAsByteArray();        
        return Converter.ToHexString(k);
    }
    public string Sign(byte[] cleartext, string key_label) 
    {
        byte[] message = cleartext;//Encoding.UTF8.GetBytes(cleartext);
        if (_session == null)
        {
            if (_logger != null)
                _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.SESSION_NOT_OPEN));
            throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.SESSION_NOT_OPEN);
        }
        try
        {
            // Prepare attribute template that defines search criteria
            List<IObjectAttribute> searchAttributes = PKCS11AttributeBuilder.PrivateKeySearch(key_label);
            // Find all objects that match provided attributes
            List<IObjectHandle> foundObjects = _session.FindAllObjects(searchAttributes);
            if (foundObjects.Count ==0)
            {
                if (_logger != null)
                    _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.MISSING_PRIVATE_KEY) + key_label);
                throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.MISSING_PRIVATE_KEY);
            }
            IObjectHandle privateKey = foundObjects[0];
            Mechanism m = new Mechanism(CKM.CKM_SHA256_RSA_PKCS);
            
            byte[] signature = _session.Sign(m, privateKey, message);
            var s= Converter.ToHexString(signature);
            return s;
        }
        catch (Pkcs11Exception e)
        {
            if (_logger != null)
                _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.SIGNING_ERROR) + key_label);
            throw HSMDriverExceptionFactory.Create(-1, HSMDriverErrorDefinitions.SIGNING_ERROR);
        }
    }
    private void OpenSession(string pwd, bool sessionRead)
    {        
        SessionType st;        
        if (sessionRead == true)
            st = SessionType.ReadOnly;
        else
            st = SessionType.ReadWrite;
        if (_library.Initialized == false)
        {
            if (_logger != null)
                _logger.LogCritical(HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.DRIVER_NOT_INITIALIZED));            
            throw HSMDriverExceptionFactory.Create(-1,HSMDriverErrorDefinitions.DRIVER_NOT_INITIALIZED);
        }
        try
        {        
            _session = _library.GetSlot.OpenSession(st); 

        }            
        catch(Exception e)
        {
            int rv = -1;
            if (e.GetType() == typeof(Pkcs11Exception))
                rv=(int)(((Pkcs11Exception)e).RV);
            CloseSession();          
            if (_logger != null)
                _logger.LogCritical(e, HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.OPEN_SESSION_ERROR));
            throw HSMDriverExceptionFactory.Create(rv, HSMDriverErrorDefinitions.OPEN_SESSION_ERROR);
        }
        try
        {
            _session.Login(CKU.CKU_USER, pwd);          
        }
        catch (Pkcs11Exception e)
        {
            if (e.RV == CKR.CKR_USER_ALREADY_LOGGED_IN)
                return;
            CloseSession();            
            if (_logger != null)
                _logger.LogCritical(e, HSMDriverErrorDefinitions.Description(HSMDriverErrorDefinitions.SESSION_LOGIN));
            throw HSMDriverExceptionFactory.Create((int)e.RV, HSMDriverErrorDefinitions.SESSION_LOGIN); 
        }

    }    
           
    
   
    

}
