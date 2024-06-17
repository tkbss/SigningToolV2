using Net.Pkcs11Interop.HighLevelAPI;

namespace Infrastructure.Pkcs11wrapper
{
    public interface IPKCS11Driver
    {
        
        void Renew();
        void UnInitialize();
        bool Initialized { get; }
        ISlot GetSlot { get; }
        string TokenName { get; set; }
        string DefaultTokenName { get; }
        void ChangeToken(string name);
    }
}
