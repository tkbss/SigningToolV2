namespace Infrastructure.Pkcs11wrapper
{
    public class HSMDriverErrorDefinitions
    {
        static public int LOAD_DRIVER                                       = 1001;
        static public int SELECT_SLOT                                       = 1002;
        static public int DRIVER_NOT_INITIALIZED                            = 1003;
        static public int OPEN_SESSION_TIMEOUT                              = 1004;
        static public int OPEN_SESSION_ERROR                                = 1005;
        static public int SESSION_LOGIN                                     = 1006;
        static public int UNKOWN_WRAP_ALGORITHM                             = 1007;
        static public int MISSING_PUBLIC_KEY                                = 1008;
        static public int PKCS_1_5_UNWRAPPING                               = 1009;
        static public int OAEP_UNWRAPPING                                   = 1010;
        static public int BASE_SESSION_KEY                                  = 1011;
        static public int COMPONENT_SECRET_EXTRACTION                       = 1012;
        static public int SESSION_KEY_VARIANT_ALGO                          = 1013;
        static public int XOR_KEY_DERIVATION_ALGO                           = 1014;
        static public int CBC_TDES_KEY_DERIVATION_ALGO                      = 1015;
        static public int MAC_COMPUTATION                                   = 1016;
        static public int SESSION_NOT_OPEN                                  = 1017;
        static public int ENC_COMPONENT_SECRET_LEN                          = 1018;
        static public int ESTABLISHED_COMPONENT_SECRET                      = 1019;
        static public int MISSING_PRIVATE_KEY                               = 1020;
        static public int WRONG_COMPONENT_SECRET                            = 1021;
        static public int PROCESSING_ERROR_COMPONENT_SECRET                 = 1022;
        static public int COMPONENT_SECRET_LEN                              = 1023;
        static public int ENC_KEY_COMPONENT_SECRET_KGNR                     = 1024;
        static public int COMPONENT_SECRET_ENCRYPTION                       = 1025;
        static public int COMPONENT_SECRET_DATA_INTEGRITY                   = 1026;
        static public int COMPONENT_SECRET_DECRYPTION                       = 1027;
        static public int DATA_ENCRYPTION                                   = 1028;
        static public int DATA_DECRYPTION                                   = 1029;
        static public int ESTABLISH_PUBLIC_KEY                              = 1030;
        static public int WRONG_EP2_SENDER_ID                               = 1031;
        static public int OAEP_WRAPPING                                     = 1032;
        static public int HKDF_DERIVATION_ALGO                              = 1033;
        static public int SIGNING_ERROR                                     = 1034;

        private const string D_LOAD_DRIVER = "Cannot load HSM driver cryptoki.dll";
        private const string D_SELECT_SLOT = "Cannot select slot in token";
        private const string D_DRIVER_NOT_INITIALIZED = "HSM driver cryptoki.dll not initialized";
        private const string D_OPEN_SESSION_TIMEOUT = "HSM session cannot be opened within a predefined timeout";
        private const string D_OPEN_SESSION_ERROR = "General error during opening HSM session";
        private const string D_SESSION_LOGIN = "Cannot login into session. Wrong password";
        private const string D_UNKOWN_WRAP_ALGORITHM = "Unkown algorithm for unwrapping session key cryptogram";
        private const string D_MISSING_PUBLIC_KEY = "Cannot find public key in token with ID : ";
        private const string D_PKCS_1_5_UNRAPPING = "Cannot unwrap session key cryptogram with algorithm PKCSV1.5 with SCS private key with ID : ";
        private const string D_OAEP_UNRAPPING = "Cannot unwrap session key cryptogram with algorithm OAEP with SCS private key with ID : ";
        private const string D_BASE_SESSION_KEY = "Cannot establish base session key in HSM from unwrapped session key cryptogram";
        private const string D_COMPONENT_SECRET_EXTRACTION = "Cannot establish component secret in HSM from unwrapped session key cryptogram";
        private const string D_SESSION_KEY_VARIANT_ALGO = "Unkown session key derivation algorithm defined";
        private const string D_XOR_KEY_DERIVATION_ALGO = "Error in XOR key derivation algorithm occurred";
        private const string D_CBC_TDES_KEY_DERIVATION_ALGO = "Error in CBC TDES key derivation algorithm occurred";
        private const string D_MAC_COMPUTATION = "Error in MAC computation occurred";
        private const string D_SESSION_NOT_OPEN = "HSM session is not open";
        private const string D_ENC_COMPONENT_SECRET_LEN = "Encrypted component secret from database has wrong length";
        private const string D_ESTABLISHED_COMPONENT_SECRET = "No component secret from session key cryptogram is established in HSM";
        private const string D_MISSING_PRIVATE_KEY = "Cannot find private signature key with ID : ";
        private const string D_WRONG_COMPONENT_SECRET = "Component secret from session key cryptogram and from database are not equal";
        private const string D_PROCESSING_ERROR_COMPONENT_SECRET = "General processing error during comparison of Component secret from session key cryptogram and from database";
        private const string D_COMPONENT_SECRET_LEN = "Component secret has wrong length";
        private const string D_ENC_KEY_COMPONENT_SECRET_KGNR = "Cannot read kgnr inside HSM for encryption key of component secret";
        private const string D_COMPONENT_SECRET_ENCRYPTION = "Error occurred during encryption key of component secret for database";
        private const string D_COMPONENT_SECRET_DATA_INTEGRITY = "Data integrity error during decryption of component secret from database";
        private const string D_COMPONENT_SECRET_DECRYPTION = "Error occurred during decryption key of component secret from database";
        private const string D_DATA_ENCRYPTION = "Error occurred during EP2 CBC data encryption";
        private const string D_DATA_DECRYPTION = "Error occurred during EP2 CBC data decryption";
        private const string D_ESTABLISH_PUBLIC_KEY = "Error in establishing public key in HSM";
        private const string D_WRONG_EP2_SENDER_ID = "Error in length of the EP2 sender ID";
        private const string D_OAEP_WRAPPING = "Cannot wrap session key with algorithm OAEP with SCS private key with ID : ";
        private const string D_HKDF_DERIVATION_ALGO = "Error in HKDF key derivation algorithm occurred";
        private const string D_SIGNING_ERROR = "Error in signature calculation";

        public static List<KeyValuePair<int, string>> ERROR_LIST = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(LOAD_DRIVER,D_LOAD_DRIVER),
            new KeyValuePair<int, string>(SELECT_SLOT,D_SELECT_SLOT),
            new KeyValuePair<int, string>(DRIVER_NOT_INITIALIZED,D_DRIVER_NOT_INITIALIZED),
            new KeyValuePair<int, string>(OPEN_SESSION_TIMEOUT,D_OPEN_SESSION_TIMEOUT),
            new KeyValuePair<int, string>(OPEN_SESSION_ERROR,D_OPEN_SESSION_ERROR),
            new KeyValuePair<int, string>(SESSION_LOGIN,D_SESSION_LOGIN),
            new KeyValuePair<int, string>(UNKOWN_WRAP_ALGORITHM,D_UNKOWN_WRAP_ALGORITHM),
            new KeyValuePair<int, string>(MISSING_PUBLIC_KEY,D_MISSING_PUBLIC_KEY),
            new KeyValuePair<int, string>(PKCS_1_5_UNWRAPPING,D_PKCS_1_5_UNRAPPING),
            new KeyValuePair<int, string>(OAEP_UNWRAPPING,D_OAEP_UNRAPPING),
            new KeyValuePair<int, string>(BASE_SESSION_KEY,D_BASE_SESSION_KEY),
            new KeyValuePair<int, string>(COMPONENT_SECRET_EXTRACTION,D_COMPONENT_SECRET_EXTRACTION),
            new KeyValuePair<int, string>(SESSION_KEY_VARIANT_ALGO,D_SESSION_KEY_VARIANT_ALGO),
            new KeyValuePair<int, string>(XOR_KEY_DERIVATION_ALGO,D_XOR_KEY_DERIVATION_ALGO),
            new KeyValuePair<int, string>(CBC_TDES_KEY_DERIVATION_ALGO,D_CBC_TDES_KEY_DERIVATION_ALGO),
            new KeyValuePair<int, string>(MAC_COMPUTATION,D_MAC_COMPUTATION),
            new KeyValuePair<int, string>(SESSION_NOT_OPEN,D_SESSION_NOT_OPEN),
            new KeyValuePair<int, string>(ENC_COMPONENT_SECRET_LEN,D_ENC_COMPONENT_SECRET_LEN),
            new KeyValuePair<int, string>(ESTABLISHED_COMPONENT_SECRET,D_ESTABLISHED_COMPONENT_SECRET),
            new KeyValuePair<int, string>(MISSING_PRIVATE_KEY,D_MISSING_PRIVATE_KEY),
            new KeyValuePair<int, string>(WRONG_COMPONENT_SECRET,D_WRONG_COMPONENT_SECRET),
            new KeyValuePair<int, string>(PROCESSING_ERROR_COMPONENT_SECRET,D_PROCESSING_ERROR_COMPONENT_SECRET),
            new KeyValuePair<int, string>(COMPONENT_SECRET_LEN,D_COMPONENT_SECRET_LEN),
            new KeyValuePair<int, string>(ENC_KEY_COMPONENT_SECRET_KGNR,D_ENC_KEY_COMPONENT_SECRET_KGNR),
            new KeyValuePair<int, string>(COMPONENT_SECRET_ENCRYPTION,D_COMPONENT_SECRET_ENCRYPTION),
            new KeyValuePair<int, string>(COMPONENT_SECRET_DATA_INTEGRITY,D_COMPONENT_SECRET_DATA_INTEGRITY),
            new KeyValuePair<int, string>(COMPONENT_SECRET_DECRYPTION,D_COMPONENT_SECRET_DECRYPTION),
            new KeyValuePair<int, string>(DATA_ENCRYPTION,D_DATA_ENCRYPTION),
            new KeyValuePair<int, string>(DATA_DECRYPTION,D_DATA_DECRYPTION),
            new KeyValuePair<int, string>(ESTABLISH_PUBLIC_KEY,D_ESTABLISH_PUBLIC_KEY),
            new KeyValuePair<int, string>(WRONG_EP2_SENDER_ID,D_WRONG_EP2_SENDER_ID),
            new KeyValuePair<int, string>(OAEP_WRAPPING,D_OAEP_WRAPPING),
            new KeyValuePair<int, string>(HKDF_DERIVATION_ALGO,D_HKDF_DERIVATION_ALGO),
            new KeyValuePair<int, string>(SIGNING_ERROR,D_SIGNING_ERROR),
        };
        public static string Description(int code) 
        {
            try 
            {
                string d = ERROR_LIST.First(item => item.Key == code).Value;
                return d;
            } 
            catch 
            {
                return string.Empty;
            }
        }
    }

}
