using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI41;

namespace Infrastructure.Pkcs11wrapper
{
    class PKCS11AttributeBuilder
    {
        
        
        public static List<IObjectAttribute> PrivateKeySearch(string keyId) 
        {
            List<IObjectAttribute> searchAttributes = new List<IObjectAttribute>
            {
                new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY),
                new ObjectAttribute(CKA.CKA_LABEL, keyId)
            };
            return searchAttributes;
        }
        public static List<IObjectAttribute> PublicKeySearch(string keyId)
        {
            List<IObjectAttribute> searchAttributes = new List<IObjectAttribute>
            {
                new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_SECRET_KEY),
                new ObjectAttribute(CKA.CKA_LABEL, keyId)
            };
            return searchAttributes;
        }





    }
}
