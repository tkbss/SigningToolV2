using Infrastructure;
using Unity;
using SoftwareSigning.Model;


namespace SoftwareSigning
{
    public interface IKeyStatusInfo
    {
        void  SetKeyStatus(string Signer, string Enviroment, string StoreType, string Origin, IUnityContainer _container);
    }
    public class KeyStatusInfoFactory
    {
        public static IKeyStatusInfo Create(KeyStatusModel status,string store_type)
        {
            STORETYPE st=Converter.ST(store_type);
            IKeyStatusInfo ks = null;
            switch(st)
            {
                case STORETYPE.KMS:
                    ks = new KMSKeyStatus(status);
                    break;
                case STORETYPE.UNMANAGED:
                    ks = new UnmanagedKeyStatus(status);
                    break;
            }
            return ks;
        }
    }
}
