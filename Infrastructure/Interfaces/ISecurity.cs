namespace Infrastructure.Interfaces
{
    public interface ISecurity
    {
        System.Security.Cryptography.X509Certificates.X509Certificate VerifyPackageSignature(PackageInfo pi);
        void CheckVerificationCert(string pwd, System.Security.Cryptography.X509Certificates.X509Certificate c);

        void GeneratePackageSignature(MANUFACTURER manufacturer,ENVIROMENT e,CERTTYPE ct, PackageInfo pi,string pwd);

        bool SigningStatus(CERTTYPE ct,ENVIROMENT e, MANUFACTURER signer, PackageInfo pi, out System.Security.Cryptography.X509Certificates.X509Certificate SigningCert);

        bool ExportSignatureExists(PackageInfo pi, CERTTYPE ct, ENVIROMENT e);

        

    }
}
