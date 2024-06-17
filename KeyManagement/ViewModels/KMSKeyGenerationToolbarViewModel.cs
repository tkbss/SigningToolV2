using Infrastructure;
using Infrastructure.Certificates;
using Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using SoftwareSigning.ViewModels;

using System.IO;

using System.Windows.Input;

namespace SigningKeyManagment.ViewModels
{
    public class KMSKeyGenerationToolbarViewModel : BindableBase
    {
        IUnityContainer _container;
        HSMStatusViewModel viewdata;
        public ICommand CreateCACertificateCommand { get; private set; }
        public ICommand CreateQACertificateCommand { get; private set; }
        public ICommand CreateATMCertificateCommand { get; private set; }
        public ICommand SignManuCertRequestCommand { get;  private set; }
        public ICommand ExportCACertificateCommand { get; private set; }
        public KMSKeyGenerationToolbarViewModel(IUnityContainer container)
        {
            _container = container;
            viewdata = container.Resolve<HSMStatusViewModel>();
            this.CreateCACertificateCommand = new DelegateCommand(this.OnCreateCACert);
            this.CreateQACertificateCommand = new DelegateCommand(this.OnCreateQACert);
            this.CreateATMCertificateCommand = new DelegateCommand(this.OnCreateATMCert);
            this.SignManuCertRequestCommand =new DelegateCommand(this.OnSignManuCertRequest);
            this.ExportCACertificateCommand = new DelegateCommand(this.OnExportCACertificate);
        }
        string key_gen_title;
        public string KeyGenTitle
        {
            get { return key_gen_title; }
            set
            {
                SetProperty(ref key_gen_title, value);
            }
        }
        bool sign_req_enabled;
        public bool SignRequestEnabled
        {
            get { return sign_req_enabled; }
            set
            {
                SetProperty(ref sign_req_enabled, value);
            }
        }
        bool create_ca_cert_enabled;
        public bool CreateCACertEnabled
        {
            get { return create_ca_cert_enabled; }
            set
            {
                SetProperty(ref create_ca_cert_enabled, value);
            }
        }
        bool create_qa_cert_enabled;
        public bool CreateQACertEnabled
        {
            get { return create_qa_cert_enabled; }
            set
            {
                SetProperty(ref create_qa_cert_enabled, value);
            }
        }
        bool create_atm_cert_enabled;
        public bool CreateATMCertEnabled
        {
            get { return create_atm_cert_enabled; }
            set
            {
                SetProperty(ref create_atm_cert_enabled, value);
            }
        }
        private void OnSignManuCertRequest()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Requests (*.der)|*.der|All files (*.*)|*.*";
            if (dialog.ShowDialog() == false)
                return;
            string cert_req_fn = dialog.FileName;
            //string pwd = viewdata.CertPwd;
            string certificate;
            try
            {
                KMSCertificates mc = new KMSCertificates();
                certificate=mc.SignManufacturerCertificate(cert_req_fn, viewdata.Enviroment, hsm);
                
            }
            
            catch (Exception e)
            {
                status.Error("SIGN PKCS10 REQUEST", e.Message);
                return;
            }
            certificate = Path.GetFileName(certificate);
            status.Success("SIGN PKCS10 REQUEST", "Request signed successfull as : " + certificate);
        }
        private void OnExportCACertificate()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Certificate (*.cer)|*.cer|All files (*.*)|*.*";
            dialog.FileName = "SIX_CA_"+ viewdata.Enviroment.ToUpper()+"_CERTIFICATE.cer";
            if (dialog.ShowDialog() == false)
                return;            
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            try
            {
                KMSCertificates ch = new KMSCertificates();
                ch.ExportCACertificate(dialog.FileName, Converter.Env(viewdata.Enviroment));
                status.Success("EXPORT CA CERTIFICATE", "CA certificate successfull exported.");
            }
            catch
            {
                status.Error("EXPORT CA CERTIFICATE", "General error during CA certificate export occurred");
            }
        }
        private void OnCreateCACert()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            
            try
            {
                Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
                KMSCertificates c = new KMSCertificates();
                c.CreateCACertificate(Converter.Env(viewdata.Enviroment), hsm);
                status.Success("CREATE CA CERTIFICATE", "CA CERTIFICATE from PUBLIC KEY in HSM created.");
            }
            catch
            {
                status.Error("CREATE CA CERTIFICATE", "CA CERTIFICATE is not created.");
            }
            HSMStatusViewModel bs = _container.Resolve<HSMStatusViewModel>();
            bs.SetUpCertStatus();
        }
        private void OnCreateQACert()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            try
            {
                KMSCertificates c = new KMSCertificates();
                Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
                c.CreateSigningCertificate(Converter.Env(viewdata.Enviroment), CERTTYPE.QA, hsm);                
                status.Success("CREATE QA CERTIFICATE", "QA CERTIFICATE from PUBLIC KEY in HSM created.");
            }
            catch
            {
                status.Error("CREATE QA CERTIFICATE", "QA CERTIFICATE is not created.");
            }
            HSMStatusViewModel bs = _container.Resolve<HSMStatusViewModel>();
            bs.SetUpCertStatus();

        }
        private void OnCreateATMCert()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            try
            {
                KMSCertificates c = new KMSCertificates();
                Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
                c.CreateSigningCertificate(Converter.Env(viewdata.Enviroment), CERTTYPE.ATM, hsm);
                status.Success("CREATE ATM CERTIFICATE", "ATM CERTIFICATE from PUBLIC KEY in HSM created.");
            }
            catch
            {
                status.Error("CREATE ATM CERTIFICATE", "ATM CERTIFICATE is not created.");
            }
            HSMStatusViewModel bs = _container.Resolve<HSMStatusViewModel>();
            bs.SetUpCertStatus();
        }
    }
}
