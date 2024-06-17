using Infrastructure;
using Infrastructure.Certificates;
using Infrastructure.HSM;
using Unity;
using SoftwareSigning.Model;
using SoftwareSigning.ViewModels;
using System.Windows.Media;

namespace SoftwareSigning
{
    class KMSKeyStatus : IKeyStatusInfo
    {
        KeyStatusModel _s;
        
        public KMSKeyStatus(KeyStatusModel status)
        {
            _s = status;
        }

        public void SetKeyStatus(string Signer, string Enviroment, string StoreType, string Origin, IUnityContainer _container)
        {
            Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
            SIXSoftwareSigningStatusBarViewModel status_bar = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            if (hsm.PasswordCheckSuccessfull == false)
            {
                _s.CAKeyStatus = new SolidColorBrush(Colors.Red);
                _s.QAKeyStatus = new SolidColorBrush(Colors.Red);
                _s.ATMKeyStatus = new SolidColorBrush(Colors.Red);
                status_bar.Error("SetupKeyStatus", "HSM Token authentication missing");
                return;
            }
            List<KEY_STATUS> status = hsm.KeyStatus(Enviroment);
            var res=status.Where(e => e.KEY_NAME.ToLower().Contains("qa") == true  && e.ESTABLISHED==true);
            if (res != null && res.Count() == 2)     
                _s.QAKeyStatus = new SolidColorBrush(Colors.Green);          
            else
                _s.QAKeyStatus = new SolidColorBrush(Colors.Red);
            res = status.Where(e => e.KEY_NAME.ToLower().Contains("ca") == true && e.KEY_NAME.ToLower().Contains(Enviroment.ToLower())==true && e.ESTABLISHED == true);
            if (res != null && res.Count() == 2)
                _s.CAKeyStatus = new SolidColorBrush(Colors.Green);
            else
                _s.CAKeyStatus = new SolidColorBrush(Colors.Red);
            res = status.Where(e => e.KEY_NAME.ToLower().Contains("sign_atm") == true && e.KEY_NAME.ToLower().Contains(Enviroment.ToLower())==true && e.ESTABLISHED == true);
            if (res != null && res.Count() == 2)
                _s.ATMKeyStatus = new SolidColorBrush(Colors.Green);
            else
                _s.ATMKeyStatus = new SolidColorBrush(Colors.Red);
            SetUpCertStatus(Enviroment);         

        }
        private void SetUpCertStatus(string enviroment)
        {
            string[] cts = { "CA", "QA", "ATM" };
            KMSCertificates certs = new KMSCertificates();
            
            foreach (string ct in cts)
            {
                CERTTYPE cert_t = Converter.CertType(ct);
                System.Security.Cryptography.X509Certificates.X509Certificate c = certs.GetCertificate(enviroment, ct);                
                string status = "CREATED";
                if (c == null)
                    status = "MISSED";
                    switch (cert_t)
                    {
                        case CERTTYPE.CA:

                            if (status == "CREATED")
                                _s.CACertStatus = new SolidColorBrush(Colors.Green);
                            else
                                _s.CACertStatus = new SolidColorBrush(Colors.Red);
                            break;
                        case CERTTYPE.QA:

                            if (status == "CREATED")
                                _s.QACertStatus = new SolidColorBrush(Colors.Green);
                            else
                                _s.QACertStatus = new SolidColorBrush(Colors.Red);
                            break;
                        case CERTTYPE.ATM:

                            if (status == "CREATED")
                                _s.ATMCertStatus = new SolidColorBrush(Colors.Green);
                            else
                                _s.ATMCertStatus = new SolidColorBrush(Colors.Red);
                            break;
                    }
            }
        }

        
    }
}
