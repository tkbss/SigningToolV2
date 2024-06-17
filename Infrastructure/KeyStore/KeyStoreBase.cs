using System.IO;

namespace Infrastructure.Certificates
{
    public class KeyStoreBase
    {
        protected string root_dir = "ATMSoftwareSigning";
        protected string root_path;
        protected string keystore_path;
        protected string env_path;

        protected const string UMD = "UNMANAGED";
        protected const string KMS = "HSM_KEY_MANAGEMENT";
        protected string[] manu;// = { "SIX", "WINCOR", "DIEBOLD", "NCR", "GLORY", "MVS", "MSIX"};        
        public KeyStoreBase()
        {
            manu = new string[Manufacturer.manu.Length + 1];
            manu[0] = "SIX";
            for(int i=1;i<manu.Length;i++)
            {
                manu[i] = Manufacturer.manu[i - 1];
            }
        }
        public string StorePath(MANUFACTURER m, ENVIROMENT e, STORETYPE st)
        {
            string m_p = RootPath();
            switch (m)
            {
                case MANUFACTURER.SIX:
                    m_p = Path.Combine(m_p, manu[0]);
                    switch (e)
                    {
                        case ENVIROMENT.TEST:
                            m_p = Path.Combine(m_p, "TEST");
                            break;
                        case ENVIROMENT.PROD:
                            m_p = Path.Combine(m_p, "PROD");
                            break;
                    }
                    switch (st)
                    {
                        case STORETYPE.UNMANAGED:
                            m_p = Path.Combine(m_p, UMD);
                            break;
                        case STORETYPE.KMS:
                            m_p = Path.Combine(m_p, KMS);
                            break;
                    }
                    break;
                case MANUFACTURER.WINCOR:
                    m_p = Path.Combine(m_p, manu[1]);
                    break;
                case MANUFACTURER.DIEBOLD:
                    m_p = Path.Combine(m_p, manu[2]);
                    break;
                case MANUFACTURER.NCR:
                    m_p = Path.Combine(m_p, manu[3]);
                    break;
                case MANUFACTURER.GLORY:
                    m_p = Path.Combine(m_p, manu[4]);
                    break;
                case MANUFACTURER.MVS:
                    m_p = Path.Combine(m_p, manu[5]);
                    break;
                case MANUFACTURER.MSIX:
                    m_p = Path.Combine(m_p, manu[6]);
                    break;
            }
            if (Directory.Exists(m_p) == false)
                Directory.CreateDirectory(m_p);

            return m_p;
        }
        public string SIXCertName(ENVIROMENT e, STORETYPE st, CERTTYPE ct)
        {
            string n = "SIX" +"_"+ Converter.Env(e)+"_" + Converter.ST(st)+"_" + Converter.CertType(ct)+".cer";
            return n;

        }
        public string StoreName(MANUFACTURER m, ENVIROMENT e, STORETYPE st, CERTTYPE ct)
        {
            string name = string.Empty;
            switch (m)
            {
                case MANUFACTURER.SIX:
                    name = manu[0];
                    break;
                case MANUFACTURER.WINCOR:
                    name = manu[1];
                    break;
                case MANUFACTURER.DIEBOLD:
                    name = manu[2];
                    break;
                case MANUFACTURER.NCR:
                    name = manu[3];
                    break;
                case MANUFACTURER.GLORY:
                    name = manu[4];
                    break;
                case MANUFACTURER.MVS:
                    name = manu[5];
                    break;
                case MANUFACTURER.MSIX:
                    name = manu[6];
                    break;
            }
            switch (e)
            {
                case ENVIROMENT.TEST:
                    name = name + "Test";
                    break;
                case ENVIROMENT.PROD:
                    name = name + "Prod";
                    break;
            }
            if (m == MANUFACTURER.SIX)
            {
                switch (st)
                {
                    case STORETYPE.UNMANAGED:
                        name = name + "Unmanaged";
                        switch (ct)
                        {
                            case CERTTYPE.CA:
                                name = name + "CAKeyStore";
                                break;
                            case CERTTYPE.QA:
                                name = name + "QAKeyStore";
                                break;
                            case CERTTYPE.ATM:
                                name = name + "ATMKeyStore";
                                break;
                            case CERTTYPE.MANU:
                                name = name + "MANUKeyStore";
                                break;
                        }
                        break;
                    case STORETYPE.KMS:
                        name = name + "KMS";
                        switch (ct)
                        {
                            case CERTTYPE.CA:
                                name = name + "CACertificate";
                                break;
                            case CERTTYPE.QA:
                                name = name + "QASigningCertificate";
                                break;
                            case CERTTYPE.ATM:
                                name = name + "ATMSigningCerificate";
                                break;
                            case CERTTYPE.MANU:
                                name = string.Empty;
                                break;
                        }
                        break;
                }
            }            
            return name;
        }
        public string CertifiedPath(MANUFACTURER m, ENVIROMENT e, STORETYPE st)
        {
            string sp = StorePath(m,e,st);
            string cf=Path.Combine(sp, "CERTIFIED");
            if (Directory.Exists(cf) == false)
                Directory.CreateDirectory(cf);
            return cf;

        }
        protected string RootPath()
        {
            string app_data = @"c:\Users\All Users\";
            root_path = Path.Combine(app_data, root_dir);            
            keystore_path = Path.Combine(root_path, "KEYSTORES");
            return keystore_path;
        }
    }

}
