using Net.Pkcs11Interop.HighLevelAPI;

namespace  Infrastructure.Pkcs11wrapper
{
    public interface IPKCS11Session
    {    
        
        void OpenReadOnlySession(string pwd);
        void OpenWriteSession(string pwd);       
        void CloseSession();
        string ReadPublicKey(string label);
        string Sign(byte[] cleartext, string key_label);
        DateTime SessionCreation 
        {
            get;
        }
        ISession HSMSession 
        {             
            get;
        }


    }
}
    

