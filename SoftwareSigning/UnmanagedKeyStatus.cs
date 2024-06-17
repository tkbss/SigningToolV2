using Infrastructure;
using Unity;
using SoftwareSigning.Model;
using SoftwareSigning.ViewModels;
using System.Windows.Media;

namespace SoftwareSigning
{
    class UnmanagedKeyStatus : IKeyStatusInfo
    {
        KeyStatusModel _s;
        public void SetKeyStatus(string Signer,string Enviroment,string StoreType,string Origin,IUnityContainer _container)
        {
            UnmanagedCertificates ch = _container.Resolve<UnmanagedCertificates>();
            SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
            toolbar.SigningEnabled = true;
            toolbar.ExportEnabled = true;
            MANUFACTURER s = Converter.Manu(Signer);
            CERTTYPE ct = Converter.CertType(Converter.SplitManuCertype(Origin)[1]);
            if (s == MANUFACTURER.SIX)
            {
                string certtype = "CA";
                KEYStoreStatus ks = ch.GetKeyStoreStatus(Converter.Env(Enviroment), Converter.ST(StoreType), Converter.CertType(certtype), Converter.Manu(Signer));
                if (ks.StoreStatus == KEYStoreStatus.CREATED)
                {
                    _s.CAKeyStatus = new SolidColorBrush(Colors.Green);
                    _s.CACertStatus= new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _s.CAKeyStatus = new SolidColorBrush(Colors.Red);
                    _s.CACertStatus = new SolidColorBrush(Colors.Red);
                }

                certtype = "QA";
                ks = ch.GetKeyStoreStatus(Converter.Env(Enviroment), Converter.ST(StoreType), Converter.CertType(certtype), Converter.Manu(Signer));
                if (ks.StoreStatus == KEYStoreStatus.CREATED)
                {
                    _s.QAKeyStatus = new SolidColorBrush(Colors.Green);
                    _s.QACertStatus = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _s.QAKeyStatus = new SolidColorBrush(Colors.Red);
                    _s.QACertStatus = new SolidColorBrush(Colors.Red);
                    if (ct == CERTTYPE.QA)
                        toolbar.SigningEnabled = false;
                }
                certtype = "ATM";
                ks = ch.GetKeyStoreStatus(Converter.Env(Enviroment), Converter.ST(StoreType), Converter.CertType(certtype), Converter.Manu(Signer));
                if (ks.StoreStatus == KEYStoreStatus.CREATED)
                {
                    _s.ATMKeyStatus = new SolidColorBrush(Colors.Green);
                    _s.ATMCertStatus = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _s.ATMKeyStatus = new SolidColorBrush(Colors.Red);
                    _s.ATMCertStatus = new SolidColorBrush(Colors.Red);
                    if (ct == CERTTYPE.ATM)
                        toolbar.SigningEnabled = false;
                }
            }
            else
            {
                KEYStoreStatus ks = ch.GetKeyStoreStatus(Converter.Env(Enviroment), Converter.ST(StoreType), CERTTYPE.MANU, Converter.Manu(Signer));
                if (ks.CertificateStatus == KEYStoreStatus.CREATED)
                {
                    _s.ManuSigningCertStatus = new SolidColorBrush(Colors.Green);
                    _s.ManuCertAvailable = true;
                }
                else
                {
                    _s.ManuSigningCertStatus = new SolidColorBrush(Colors.Red);
                    toolbar.SigningEnabled = false;
                    _s.ManuCertAvailable = false;
                }
                if (ks.StoreStatus == KEYStoreStatus.CREATED)
                    _s.ManuSigningKeyStatus = new SolidColorBrush(Colors.Green);
                else
                {
                    _s.ManuSigningKeyStatus = new SolidColorBrush(Colors.Red);
                    toolbar.SigningEnabled = false;
                }
                if (ks.RequestStatus == KEYStoreStatus.CREATED)
                    _s.ManuCertReqStatus = new SolidColorBrush(Colors.Green);
                else
                    _s.ManuCertReqStatus = new SolidColorBrush(Colors.Red);
            }
        }
        public UnmanagedKeyStatus(KeyStatusModel status)
        {
            _s = status;
        }
    }
}
