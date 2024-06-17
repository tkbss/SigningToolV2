namespace Infrastructure
{


    public interface IUnmanagedCertificates
    {
        bool CheckCAKeys(string pwd);   
        void CreateCACertificate(string cn, string ou, string o, string c, string pwd);
        void CreateManufacturerSigningKeys(string manu,string env, string pwd, string cn, string ou, string o, string c);

        void CreateSIXSigningCertificate(string cn, string ou, string o, string c, string type,string pwd);
        string SignManufacturerCertificate(string manu, string cert_req,string pwd,out string sn_cert);

        void ExportManufacturerSigningRequest(string manu,string env, string pwd, string target_fn);

        System.Security.Cryptography.X509Certificates.X509Certificate GetCertificate(MANUFACTURER m,ENVIROMENT e, CERTTYPE ct, string pwd);

        KEYStoreStatus GetKeyStoreStatus(ENVIROMENT env, STORETYPE m, CERTTYPE t, MANUFACTURER manu);        
        string GetSIXKeyStorePath(string type,string env);
        string GetStoreName(MANUFACTURER m, ENVIROMENT e, STORETYPE st, CERTTYPE ct);
        void ExportCACertificate(string target_fn, string pwd);
        /*
        string GetStorePassword(MANUFACTURER m);
        void SetStorePassword(MANUFACTURER m, string pwd);
        void DeleteStorePassword(MANUFACTURER m);
        void ChangeStorePassword(MANUFACTURER m, CERTTYPE ct, string oldp, string newp);
        */
    }
}
