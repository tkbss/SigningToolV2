using Infrastructure.Pkcs11wrapper;
using Net.Pkcs11Interop.HighLevelAPI;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Infrastructure.HSM
{
    class InteropHSM
    {
        [DllImport("msvcrt.dll")]
        private static extern int _putenv_s(string e, string v);
        public static void OverrideHsmParameter(string regKey, string parameter, string value, bool viaRegistry)
        {
            _putenv_s(parameter, value);
            Environment.SetEnvironmentVariable(parameter, value, EnvironmentVariableTarget.Process);
        }
        private static IPKCS11Session? _session;
        public static  int Initialize() 
        { 
            IPKCS11Driver pkcs11 = PKCS11Driver.GetInstance();
            return 0;
        }
        public static  int OpenSession(string pwd,string slot_name,int slot_number) 
        {
            _session = new PKCS11Session(logger: null, driverLogger: null);
            _session.OpenReadOnlySession(pwd);
            return 0;
        }
        public static  int CloseSession() 
        { 
            if(_session != null)
            {
                _session.CloseSession();
            }     
            _session = null;
            return 0; 
        }
        public static bool KeyStatus(string key_label, bool priv_key)
        {
            if(_session == null)
                return false;
            try
            {
                if (priv_key)
                {
                    List<IObjectAttribute> searchAttributes = PKCS11AttributeBuilder.PrivateKeySearch(key_label);
                    // Find all objects that match provided attributes
                    List<IObjectHandle> foundObjects = _session.HSMSession.FindAllObjects(searchAttributes);
                    if (foundObjects.Count == 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    List<IObjectAttribute> searchAttributes = PKCS11AttributeBuilder.PublicKeySearch(key_label);
                    // Find all objects that match provided attributes
                    List<IObjectHandle> foundObjects = _session.HSMSession.FindAllObjects(searchAttributes);
                    if (foundObjects.Count == 0)
                        return false;
                    else
                        return true;
                }
            }
            catch
            {
                return false;
            }
            
        }
        public static string ReadPublicKey(string label) 
        {
            if (_session == null)
                return string.Empty;
            return _session.ReadPublicKey(label); 
        }
        public static  string Sign(byte[] cleartext,string key_label)
        {   if (_session == null)
                    return string.Empty;
            return _session.Sign(cleartext,key_label);
        }
        [DllImport("InteropHSM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Finalize();

        
        

        
        

        
        

        
        

        [DllImport("InteropHSM.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static extern string SlotArray(
                [param: MarshalAs(UnmanagedType.LPWStr)] String token_name
            );
        
        
    }
}
