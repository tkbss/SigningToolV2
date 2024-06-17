using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoftwareSigning.Model
{
    public class KeyStatusModel:  BindableBase
    {
        Brush ca_cert_status;
        public Brush CACertStatus
        {
            get { return ca_cert_status; }
            set
            {
                SetProperty(ref ca_cert_status, value);
            }
        }
        Brush qa_cert_status;
        public Brush QACertStatus
        {
            get { return qa_cert_status; }
            set
            {
                SetProperty(ref qa_cert_status, value);
            }
        }
        Brush atm_cert_status;
        public Brush ATMCertStatus
        {
            get { return atm_cert_status; }
            set
            {
                SetProperty(ref atm_cert_status, value);
            }
        }
        Brush manu_signing_key_status;
        public Brush ManuSigningKeyStatus
        {
            get { return manu_signing_key_status; }
            set
            {
                SetProperty(ref manu_signing_key_status, value);
            }
        }
        Brush cakey_status;
        public Brush CAKeyStatus
        {
            get { return cakey_status; }
            set
            {
                SetProperty(ref cakey_status, value);
            }
        }
        Brush qakey_status;
        public Brush QAKeyStatus
        {
            get { return qakey_status; }
            set
            {
                SetProperty(ref qakey_status, value);
            }
        }
        Brush atmkey_status;
        public Brush ATMKeyStatus
        {
            get { return atmkey_status; }
            set
            {
                SetProperty(ref atmkey_status, value);
            }
        }
        Brush manu_signing_cert_status;
        public Brush ManuSigningCertStatus
        {
            get { return manu_signing_cert_status; }
            set
            {
                SetProperty(ref manu_signing_cert_status, value);
            }
        }
        Brush manu_cert_req_status;
        public Brush ManuCertReqStatus
        {
            get { return manu_cert_req_status; }
            set
            {
                SetProperty(ref manu_cert_req_status, value);
            }
        }
        public bool ManuCertAvailable { get; set; }
    }
}
