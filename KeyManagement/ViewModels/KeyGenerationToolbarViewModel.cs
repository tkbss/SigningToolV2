using Infrastructure;
using Infrastructure.Exceptions;
using Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using SoftwareSigning.ViewModels;

using System.IO;

using System.Windows.Input;
using SoftwareSigning.Model;

namespace SigningKeyManagment.ViewModels
{
    public class KeyGenerationToolbarViewModel: BindableBase
    {
        
        public ICommand CreateCAKeysCommand { get; private set; }
        public ICommand CreateQAKeysCommand { get; private set; }
        public ICommand CreateATMKeysCommand { get; private set; }
        public ICommand CreateManuKeysCommand { get; private set; }
        public ICommand CreatePKCS10RequestCommand { get; private set; }
        public ICommand SignRequestCommand { get; private set; }
        public ICommand ImportManufacturerCertificateCommand { get; private set; }
        public ICommand ExportCACertCommand { get; private set; }

        public ICommand ChangeManuStorePwdCommand { get; private set; }

        IUnityContainer _container;
        KeyGenerationViewModel viewdata;
        UnmanagedCertificates ch;
        string toolbar_cmd;
        public string ToolbarCmd
        {
            get { return toolbar_cmd; }
            set
            {
                SetProperty(ref toolbar_cmd, value);
            }
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
        string cmd_messsage;
        public string CmdMessage
        {
            get { return cmd_messsage; }
            set
            {
                SetProperty(ref cmd_messsage, value);
            }
        }
        bool manu_key_gen_enabled;
        public bool ManuKeyGenEnabled
        {
            get { return manu_key_gen_enabled; }
            set
            {
                SetProperty(ref manu_key_gen_enabled, value);
            }
        }
        bool ca_key_gen_enabled;
        public bool CAKeyGenEnabled
        {
            get { return ca_key_gen_enabled; }
            set
            {
                SetProperty(ref ca_key_gen_enabled, value);
            }
        }
        bool atm_key_gen_enabled;
        public bool ATMKeyGenEnabled
        {
            get { return atm_key_gen_enabled; }
            set
            {
                SetProperty(ref atm_key_gen_enabled, value);
            }
        }
        bool qa_key_gen_enabled;
        public bool QAKeyGenEnabled
        {
            get { return qa_key_gen_enabled; }
            set
            {
                SetProperty(ref qa_key_gen_enabled, value);
            }
        }
        public KeyGenerationToolbarViewModel(IUnityContainer container)
        {
            _container = container;
            viewdata=container.Resolve<KeyGenerationViewModel>();
            ch = _container.Resolve<UnmanagedCertificates>();
            this.CreateCAKeysCommand = new DelegateCommand(this.OnCreateCAKeys);
            this.CreateQAKeysCommand = new DelegateCommand(this.OnCreateQAKeys);
            this.CreateATMKeysCommand = new DelegateCommand(this.OnCreateATMKeys);
            this.CreateManuKeysCommand = new DelegateCommand(this.OnCreateManuKeys);
            this.CreatePKCS10RequestCommand = new DelegateCommand(this.OnCreatePKCS10Request);
            this.SignRequestCommand = new DelegateCommand(this.OnSignRequest);
            this.ImportManufacturerCertificateCommand = new DelegateCommand(this.OnImportManufacturerCertificate);
            this.ExportCACertCommand = new DelegateCommand(this.OnExportCACertificate);
            this.ChangeManuStorePwdCommand = new DelegateCommand(this.OnChangeManuStorePwd);
        }
        private void OnChangeManuStorePwd()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string[] m_c = Converter.SplitManuCertype(viewdata.Origin);
            MANUFACTURER m = Converter.Manu(m_c[0]);
            ENVIROMENT e = Converter.Env(viewdata.Enviroment);
            CERTTYPE ct = Converter.CertType(m_c[1]);
            if(m==MANUFACTURER.SIX)
            {
                status.Success("CHANGE STORE PWD", "Password for SIX stores are not changed.");
                return;
            }
            if (string.IsNullOrEmpty(viewdata.OldManuStorePwd)==true)
            {
                status.Error("CHANGE STORE PWD", "Enter old password.");
                return;
            }
            if (string.IsNullOrEmpty(viewdata.NewManuStorePwd) == true)
            {
                status.Error("CHANGE STORE PWD", "Enter new password.");
                return;
            }                           
            try
            {
                StorePasswordSafe pwd_m = _container.Resolve<StorePasswordSafe>();
                pwd_m.ChangeStorePassword(m,e, ct, viewdata.OldManuStorePwd, viewdata.NewManuStorePwd);                
                pwd_m.PasswordChanged = true;
                status.Success("CHANGE STORE PWD", "Store password changed successfull.");                
                pwd_m.SetStorePassword(m,e,viewdata.NewManuStorePwd);
                viewdata.OldManuStorePwd = string.Empty;
                viewdata.NewManuStorePwd = string.Empty;
                return;
            }
            catch(StorePasswordExceptions)
            {
                status.Error("CHANGE STORE PWD", "Wrong old password.");
                viewdata.OldManuStorePwd = string.Empty;
                viewdata.NewManuStorePwd = string.Empty;
                return;
            }
            catch(Exception ex)
            {
                status.Error("CHANGE STORE PWD", "General processing error. "+ex.Message);
                viewdata.OldManuStorePwd = string.Empty;
                viewdata.NewManuStorePwd = string.Empty;
                return;
            }
            
            

        }
        private void OnExportCACertificate()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Certificate (*.cer)|*.cer|All files (*.*)|*.*";
            dialog.FileName = "SIX_CA_CERTIFICATE.cer";
            if (dialog.ShowDialog() == false)
                return;
            string pwd = "1234";
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            try
            {
                ch.ExportCACertificate(dialog.FileName, pwd);
                status.Success("EXPORT CA CERTIFICATE", "CA certificate successfull exported.");
            }
            catch
            {
                status.Error("EXPORT CA CERTIFICATE", "General error during CA certificate export occurred");
            }
        }
        private void OnImportManufacturerCertificate()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string pwd = GetPassword();
            if (string.IsNullOrEmpty(pwd) == true)
            {
                status.Error("IMPORT MANU CERTIFICATE", "Missing store password");
                return;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Requests (*.cer)|*.cer|All files (*.*)|*.*";
            if (dialog.ShowDialog() == false)
                return;
            string cert_path = dialog.FileName;
            try
            {
                ch.ImportManufacturerSigningCertificate(viewdata.Manufacturer,viewdata.Enviroment, pwd, cert_path);
            }
            catch(StorePasswordExceptions)
            {
                status.Error("IMPORT MANU CERTIFICATE", "Wrong Store password");
                return;
            }
            catch(Exception e)
            {
                status.Error("IMPORT MANU CERTIFICATE", e.Message);
                return;
            }
            viewdata.SetManuKeyStatus(viewdata.Manufacturer,viewdata.Enviroment);            
            status.Success("IMPORT MANU CERTIFICATE", "Certificate imported successfull");
        }
        
        private void OnSignRequest()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SIXSoftwareSigningViewModel sig_view = _container.Resolve<SIXSoftwareSigningViewModel>();           
            string cert_req_fn= viewdata.ManuCertRequest;
            if (string.IsNullOrEmpty(viewdata.ManuCertRequest)==true)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                status.Error("SIGN PKCS10 REQUEST", "No PKCS10 request available");
                dialog.Filter = "Requests (*.der)|*.der|All files (*.*)|*.*";
                if (dialog.ShowDialog() == false)
                    return;
                cert_req_fn = dialog.FileName; 
            }           
            string pwd = GetPassword();
            string certificate;            
            try
            {
                string sn_cert = string.Empty;
                certificate=ch.SignManufacturerCertificate(viewdata.Manufacturer, cert_req_fn, pwd,out sn_cert);
                sig_view.PackageProvider = sn_cert;
            }
            catch(StorePasswordExceptions)
            {
                status.Error("SIGN PKCS10 REQUEST", "Wrong CA Store password");
                return;
            }
            catch(Exception e)
            {
                status.Error("SIGN PKCS10 REQUEST", e.Message);
                sig_view.ErrorMessage = e.Message;                
                return;
            }
            certificate=Path.GetFileName(certificate);
            status.Success("SIGN PKCS10 REQUEST", "Request signed successfull as : "+certificate);
            
        }
        private void OnCreatePKCS10Request()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string pwd = GetPassword();
            if(string.IsNullOrEmpty(pwd)==true)
            {
                status.Error("STORE PASSWORD", "Missing store password");
                return;
            }            
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Request (*.der)|*.der|All files (*.*)|*.*";
            string fn = viewdata.Manufacturer + "_" + viewdata.Enviroment+"_PKCS10";
            dialog.FileName = fn;
            if (dialog.ShowDialog() == false)
                return;
           
            string tp = dialog.FileName;
            SIXSoftwareSigningViewModel sig_view= _container.Resolve<SIXSoftwareSigningViewModel>();
            SoftwareSigningToolbarViewModel sig_tb= _container.Resolve<SoftwareSigningToolbarViewModel>();
            try
            {
                ch.ExportManufacturerSigningRequest(viewdata.Manufacturer,viewdata.Enviroment, pwd, tp);
                status.Success("PKCS10 REQUEST", "Successfull created and exported");
                sig_tb.Log(LogData.OPERATION.PKCS10_REQ, LogData.RESULT.SUCCESS, sig_view);
            }
            catch(StorePasswordExceptions)
            {
                status.Error("PKCS10 REQUEST", "Wrong store password");
            }
            catch(Exception e)
            {
                status.Error("PKCS10 REQUEST", e.Message);
                sig_tb.ErrorMessage = e.Message;
                sig_tb.Log(LogData.OPERATION.PKCS10_REQ, LogData.RESULT.ERROR, sig_view);
            }

        }
        private void OnCreateManuKeys()
        {
            if(ManuInputDataValidation()==false)
            {
                return;
            }
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            try
            {
                StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();
                string pwd = pwds.GetStorePassword(Converter.Manu(viewdata.Manufacturer),Converter.Env(viewdata.Enviroment));
                ch.CreateManufacturerSigningKeys(viewdata.Manufacturer,viewdata.Enviroment, pwd, viewdata.DNSName.SubjectCN, viewdata.DNSName.SubjectOU, viewdata.DNSName.SubjectO, viewdata.DNSName.SubjectC);
                status.Success("MANU SIGNING KEY GENERATION", "KEYS successfull generated");
            }
            catch(Exception e)
            {
                status.Error("MANU SIGNING KEY GENERATION", "General error during key generation.");
            }
            viewdata.SetManuKeyStatus(viewdata.Manufacturer,viewdata.Enviroment);
           
            viewdata.ChkAbort = true;
        }
        private void OnCreateATMKeys()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string CertPwd = "1234";
            if (SIXInputDataValidation(viewdata.ATMKeyStatus,CERTTYPE.ATM) == false)
            {
                status.Error("ATM KEY DATA VALIDATION", CmdMessage);
                return;
            }
            if (ch.CheckCAKeys(CertPwd) == false)
            {
                CmdMessage = "CA KEYS ARE NOT SETUP. CANNOT GENERATE OTHER CERTIFICATES";
                status.Error("ATM KEY DATA VALIDATION", CmdMessage);
                return;
            }
            ch.CreateSIXSigningCertificate(viewdata.DNSName.SubjectCN, viewdata.DNSName.SubjectOU, viewdata.DNSName.SubjectO, viewdata.DNSName.SubjectC, ch.SIXSATMSigningKeys, CertPwd);
            if (viewdata.ChkBackup == true)
            {
                CmdMessage = "OLD ATM KEYS AND CERTIFICATE BACKUP DONE. NEW ATM KEYS AND CERTIFICATE CREATED SUCCESSFULL";
            }
            if (viewdata.ChkOverride == true)
            {
                CmdMessage = "OVEWRITTEN OLD ATM KEYS AND CERTIFICATE. ATM CERTIFICATE CREATED SUCCESSFULL";
            }
            else
                CmdMessage = "ATM CERTIFICATE CREATED SUCCESSFULL";
            status.Success("ATM KEY GENERATION", CmdMessage);
            ResetInitialValues();
            viewdata.SetSIXKeyStatus("SIX", "TEST", "UNMANAGED");
        }
        private void OnCreateQAKeys()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string CertPwd = "1234";
            if (SIXInputDataValidation(viewdata.QAKeyStatus,CERTTYPE.QA) == false)
            {
                status.Error("QA KEY DATA VALIDATION", CmdMessage);
                return;
            }
            if (ch.CheckCAKeys(CertPwd) == false)
            {
                CmdMessage = "CA KEYS ARE NOT SETUP. CANNOT GENERATE OTHER CERTIFICATES";
                status.Error("QA KEY DATA VALIDATION", CmdMessage);
                return;
            }
            ch.CreateSIXSigningCertificate(viewdata.DNSName.SubjectCN, viewdata.DNSName.SubjectOU, viewdata.DNSName.SubjectO, viewdata.DNSName.SubjectC, ch.SIXSQASigningKeys, CertPwd);
            if (viewdata.ChkBackup == true)
            {
                CmdMessage = "OLD QA KEYS AND CERTIFICATE BACKUP DONE. NEW QA KEYS AND CERTIFICATE CREATED SUCCESSFULL";
            }
            if (viewdata.ChkOverride == true)
            {
                CmdMessage = "OVEWRITTEN OLD QA KEYS AND CERTIFICATE. QA CERTIFICATE CREATED SUCCESSFULL";
            }
            else
                CmdMessage = "QA CERTIFICATE CREATED SUCCESSFULL";
            status.Success("QA KEY GENERATION", CmdMessage);
            ResetInitialValues();
            viewdata.SetSIXKeyStatus("SIX", "TEST", "UNMANAGED");

        }
        private void OnCreateCAKeys()
        {
            SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            string CertPwd = "1234";
            
            if (SIXInputDataValidation(viewdata.CAKeyStatus,CERTTYPE.CA) == false)
            {
                status.Error("CA KEY DATA VALIDATION", CmdMessage);
                return;
            }            
            ch.CreateCACertificate(viewdata.DNSName.CASubjectCN, viewdata.DNSName.CASubjectOU, viewdata.DNSName.CASubjectO, viewdata.DNSName.CASubjectC, CertPwd);
            if (viewdata.ChkBackup == true)
            {                
                CmdMessage = "OLD CA KEYS AND CERTIFICATE BACKUP DONE. NEW CA KEYS AND CERTIFICATE CREATED SUCCESSFULL";
            }
            if (viewdata.ChkOverride == true)
            {
                CmdMessage = "OVEWRITTEN OLD CA KEYS AND CERTIFICATE. CA CERTIFICATE CREATED SUCCESSFULL";
            }
            else
                CmdMessage = "CA CERTIFICATE CREATED SUCCESSFULL";
            status.Success("CA KEY GENERATION", CmdMessage);
            ResetInitialValues();
            viewdata.SetSIXKeyStatus("SIX", "TEST", "UNMANAGED");
        }
        private bool SIXInputDataValidation(string status,CERTTYPE ct)
        {
            if (viewdata.DNSName.IsEmpty()==true)
            {

                CmdMessage = "SUBJECT DNS NAME NOT SET";
                return false;
            }
            
            //if (string.IsNullOrEmpty(CertPwd) == true)
            //{
            //    CmdMessage = "PASSWORD TO PROTECT PRIVATE KEY IS MISSING";
            //    return false;
            //}
            if (status == KEYStoreStatus.CREATED)
            {
                
                if (viewdata.ChkBackup == true)
                {
                    //do backup.
                    viewdata.BackupStore(ct);
                }
            }
            
            return true;
        }
        private bool ManuInputDataValidation()
        {
            string pwd = GetPassword();
            if (string.IsNullOrEmpty(pwd) == true)
            {
                //No password for store available
                SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
                status.Error("PASSWORD VALIDATION", "Missing password for key store.");
                return false;
            }
            if (viewdata.DNSName.IsEmpty()==true)
            {
                SIXSoftwareSigningStatusBarViewModel status = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
                status.Error("DATA VALIDATION", "SUBJECT DNS NAME NOT SET");
                return false;
            }
            if (viewdata.ChkBackup == true)
            {
                //do backup.
                viewdata.BackupStore(CERTTYPE.MANU);
            }
            return true;
        }
       
        
        private string GetPassword()
        {
            string pwd = string.Empty;
            MANUFACTURER m = Converter.Manu(viewdata.Manufacturer);
            ENVIROMENT e = Converter.Env(viewdata.Enviroment);
            StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();
            pwd = pwds.GetStorePassword(m, e);
            //if (string.IsNullOrEmpty(viewdata.CertPwd) == true)
            //{
            //    pwd = pwds.GetStorePassword(m,e);
            //}
            //else
            //{
            //    pwd = viewdata.CertPwd;
            //    pwds.SetStorePassword(m,e, pwd);
            //}
            return pwd;
        }
        private void ResetInitialValues()
        {
            viewdata.ChkAbort = true;
            //viewdata.DNSName.SubjectCN = string.Empty;
        }
    }
}
