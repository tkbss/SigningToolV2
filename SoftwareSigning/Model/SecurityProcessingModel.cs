using Infrastructure;
using Infrastructure.Certificates;
using Infrastructure.Exceptions;
using Unity;
using SoftwareSigning.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TracingModule;

namespace SoftwareSigning.Model
{
    public class SecurityProcessingModel
    {
        IUnityContainer container;
        string ErrorMessage;
        public void DetermineSigningStatus(IUnityContainer _container)
        {
            SIXSoftwareSigningViewModel vm = _container.Resolve<SIXSoftwareSigningViewModel>();
            vm.ExportSignatureExists = "False";
            PackageSigningStatus(vm, _container);
            ATMSigningStatus(vm, _container);
        }
        public void SignatureVerification(IUnityContainer _container)
        {
            container = _container;         
            SIXSoftwareSigningViewModel vm = _container.Resolve<SIXSoftwareSigningViewModel>();
            SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();
            SignerCertificateMapping cert_owner = _container.Resolve<SignerCertificateMapping>();
            SIGNER s = Converter.Signer(vm.Origin);
            if (s==SIGNER.MANU)
            {
                //SIGNER=MANU => PACKAGE contains no signature. Everything OK. Nothing to verify
                vm.PackageVerification = true;                
                sbvm.Success("PACKAGE LOAD", "SELECTED VERSION SUCCESSFULL LOADED.");                
                return;
            }
            //vm.SecurityInfo.Clear();
            if (vm.PI.Security==null || vm.PI.Security.Count() == 0)
            {
                //NO SIGNING INFO AVAILABLE 
                ErrorMessage = "Package contains no signature. Package signing not allowed";
                Log(LogData.OPERATION.VERIFICATION, LogData.RESULT.P_VERIFY_ERROR, vm);
                sbvm.Error("PACKAGE SIGNATURE VERIFICATION",ErrorMessage );
                vm.PackageVerification = false;
                return;                
            }
            bool md_validation = true;            
            foreach (SecurityInfo si in vm.PI.Security)
            {
                PackageSecurityInfoModel sim = new PackageSecurityInfoModel();
                sim.Algorithm = si.Algorithm;
                sim.Digest = si.Digest;
                sim.ComputedDigest = si.Digest;
                sim.FileName = si.FileName;
                sim.ComputedDigest = si.ComputedDigest;
                if (sim.Digest == sim.ComputedDigest)
                    sim.MDStatus = new SolidColorBrush(Colors.Green);
                else
                {
                    sim.MDStatus = new SolidColorBrush(Colors.Red);
                    md_validation = false;                    
                }
                vm.SecurityInfo.Add(sim);
            }            
            SIGNER v = SIGNER.MANU;
            try
            {
                string pwd = "1234";
                vm.VerificationCertificate = sec.VerifyPackageSignature(vm.StoreType, vm.PI.GetPackageInfo(), vm.Enviroment,pwd);
                vm.SignatureStatus = new SolidColorBrush(Colors.Green);
                vm.CertifcateValidityStatus = new SolidColorBrush(Colors.Green);                
                vm.VerificationCertificateOwner = cert_owner.ResolveSigner(vm.VerificationCertificate, Converter.Env(vm.Enviroment), Converter.ST(vm.StoreType),out v);
                if (string.IsNullOrEmpty(vm.VerificationCertificateOwner) == true)
                    vm.VerificationCertificateOwner = "UNKNOWN";             
            }
            catch (SignatureVerificationException )
            {
                //wirte error to status bar
                ErrorMessage = "Signature validation failed. Package signing is not possible.";
                Log(LogData.OPERATION.VERIFICATION, LogData.RESULT.P_VERIFY_ERROR, vm);
                sbvm.Error("PACKAGE SIGNATURE VERIFICATION", "Signature validation failed. Package signing is not possible.");
                vm.SignatureStatus = new SolidColorBrush(Colors.Red);
                vm.CertifcateValidityStatus = new SolidColorBrush(Colors.Red);
                vm.VerificationCertificate = sec.Verifier;
                vm.PackageVerification = false;
                toolbar.SigningEnabled = false;
                return;
            }
            catch (CertificateValidationException )
            {
                ErrorMessage = "Signature key validation failed. Package signing is not possible.";
                Log(LogData.OPERATION.VERIFICATION, LogData.RESULT.P_VERIFY_ERROR, vm);
                sbvm.Error("PACKAGE SIGNATURE VERIFICATION", "Signature key validation failed. Package signing is not possible.");
                vm.SignatureStatus = new SolidColorBrush(Colors.Green);
                vm.CertifcateValidityStatus = new SolidColorBrush(Colors.Red);
                vm.VerificationCertificate = sec.Verifier;
                vm.PackageVerification = false;
                toolbar.SigningEnabled = false;
                return;
            }
            if (md_validation == false)
            {
                ErrorMessage = "Message digests in setup.info are not correct. Package signing is not possible.";
                Log(LogData.OPERATION.VERIFICATION, LogData.RESULT.P_VERIFY_ERROR, vm);
                sbvm.Error("MESSAGE DIGEST VALIDATION", "Message digests in setup.info are not correct. Package signing is not possible.");
                vm.PackageVerification = false;
                toolbar.SigningEnabled = false;
                return;
            }
            else
            {
                ErrorMessage = string.Empty;
                Log(LogData.OPERATION.VERIFICATION, LogData.RESULT.P_VERIFY_SUCCESS, vm);
                sbvm.Success("PACKAGE VERIFICATION", "Signature and message digests in package are correct. Package signing is possible.");
                vm.PackageVerification = true;
                CheckVerifcationCertOwner(s, vm,sbvm,toolbar,v);                
            }

        }
        private void CheckVerifcationCertOwner(SIGNER s, SIXSoftwareSigningViewModel vm, SIXSoftwareSigningStatusBarViewModel sbvm, SoftwareSigningToolbarViewModel toolbar,SIGNER v)
        {
            if (s == SIGNER.MANU)
                return;
            if (vm.VerificationCertificateOwner == "UNKNOWN")
            {
                vm.ErrorMessage = "Signature verifcation ok but certificate owner unknown. Package signed by wrong instance.";
                sbvm.Error("PACKAGE VERIFICATION",vm.ErrorMessage);
                return;
            }
            if (s==SIGNER.QA)
            {
                
                if (v == SIGNER.MANU)
                    return;
                else
                {
                    vm.PackageVerification = false;
                    vm.ErrorMessage = "Package signed by wrong Package provider.";
                    sbvm.Error("PACKAGE VERIFICATION", vm.ErrorMessage);
                    toolbar.SigningEnabled = false;
                    return;
                }
            }
            if(s==SIGNER.ATM)
            {
                PackageProcessing pp = new PackageProcessing();
                STORETYPE st = Converter.ST(vm.StoreType);
                ENVIROMENT e = Converter.Env(vm.Enviroment);
                if (e==ENVIROMENT.PROD)
                {
                    if (v != SIGNER.ATM)
                    {
                        vm.PackageVerification = false;
                        vm.ErrorMessage = "Package signed by wrong Package provider. Signing required by ATM-TEST";
                        sbvm.Error("PACKAGE VERIFICATION", vm.ErrorMessage);
                        toolbar.SigningEnabled = false;
                        pp.RemovePackage(Converter.Manu(vm.PackageProvider), SIGNER.ATM, st, e, vm.PI.Version, vm.PI.Name);
                        return;
                    }
                    return;
                }               
                if(v!=SIGNER.QA)
                {
                    vm.PackageVerification = false;
                    vm.ErrorMessage = "Package signed by wrong Package provider.";
                    sbvm.Error("PACKAGE VERIFICATION",vm.ErrorMessage );
                    toolbar.SigningEnabled = false;                    
                    pp.RemovePackage(Converter.Manu(vm.PackageProvider), SIGNER.ATM,st,e, vm.PI.Version, vm.PI.Name);
                    return;
                }
            }
        }
        public void Log(LogData.OPERATION op, LogData.RESULT res, SIXSoftwareSigningViewModel signing)
        {
            LogData log = container.Resolve<LogData>();

            log.Operation = op;
            log.OperationResult = res;
            log.e = Converter.Env(signing.Enviroment);
            log.Signer = Converter.Manu(signing.Signer);
            log.SignerType = Converter.Signer(signing.SignerType);
            log.StoreType = Converter.ST(signing.StoreType);
            log.ErrorMessage = ErrorMessage;
            int index = log.Log();
            if (signing.PI != null)
            {
                TracePackageData pd = new TracePackageData();
                pd.Date = signing.PI.PackageDate;
                pd.PackageName = signing.PI.Name;
                pd.Vendor = signing.PI.Vendor;
                pd.Version = signing.PI.Version;
                log.LogPackage(pd, index);
            }
        }
        public void ATMSigningStatus(SIXSoftwareSigningViewModel vm, IUnityContainer _container)
        {
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();            
            if (vm.PI == null)
            {
                vm.ATMSigningStatus= new SolidColorBrush(Colors.Red);
                vm.ATMExportEnabled = false;
                return;
            }
            PackageInfo pi = vm.PI.GetPackageInfo();
            if (sec.ExportSignatureExists(Converter.ST(vm.StoreType), Converter.Env(vm.Enviroment), CERTTYPE.ATM, pi) == true)
            {
                vm.ATMSigningStatus = new SolidColorBrush(Colors.Green);
                vm.ATMExportEnabled = true;
            }
            else
            {
                vm.ATMSigningStatus = new SolidColorBrush(Colors.Red);
                vm.ATMExportEnabled = false;
            }
        }
        public void PackageSigningStatus(SIXSoftwareSigningViewModel vm, IUnityContainer _container)
        {
            SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();
            vm.SignerCertificateOwner = "UNKNOWN";
            if (vm.PI == null || vm.PackageVerification == false)
            {
                vm.PackageVerification = false;
                toolbar.SigningEnabled = false;
                toolbar.ExportEnabled = false;
                vm.SigningStatus = new SolidColorBrush(Colors.Red);
                vm.SignatureStatus = new SolidColorBrush(Colors.Red);
                vm.CertifcateValidityStatus = new SolidColorBrush(Colors.Red);
                if (vm.PI == null)
                    return;
            }
            else
            {
                vm.SignatureStatus = new SolidColorBrush(Colors.Green);
                vm.CertifcateValidityStatus = new SolidColorBrush(Colors.Green);
                toolbar.SigningEnabled = true;
            }
            PackageInfo pi = vm.PI.GetPackageInfo();
            vm.ExportSignatureExists = sec.ExportSignatureExists(Converter.ST(vm.StoreType), Converter.Env(vm.Enviroment),Converter.CertType(vm.SignerType), pi).ToString();           
            System.Security.Cryptography.X509Certificates.X509Certificate s_c = null;
            bool r = sec.SigningStatus(Converter.ST(vm.StoreType), Converter.Manu(vm.Signer), Converter.Env(vm.Enviroment), Converter.CertType(vm.SignerType), pi, out s_c);
            if (r == true)
            {
                vm.SigningCertificate = s_c;                
                SignerCertificateMapping cert_owner = _container.Resolve<SignerCertificateMapping>();
                SIGNER v = SIGNER.MANU;
                vm.SignerCertificateOwner = cert_owner.ResolveSigner(vm.SigningCertificate, Converter.Env(vm.Enviroment), Converter.ST(vm.StoreType),out v);
                if (string.IsNullOrEmpty(vm.SignerCertificateOwner) == true)
                {
                    vm.SignerCertificateOwner = "UNKNOWN";
                    vm.SigningStatus = new SolidColorBrush(Colors.Red);
                }
                else
                    vm.SigningStatus = new SolidColorBrush(Colors.Green);
                toolbar.ExportEnabled = true;
                return;
            }
            else
            {
                vm.SigningCertificate = null;
                vm.SigningStatus = new SolidColorBrush(Colors.Red);
                toolbar.ExportEnabled = false;
                return;
            }


        }
    }
}

