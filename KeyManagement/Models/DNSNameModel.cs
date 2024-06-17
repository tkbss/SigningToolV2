using Infrastructure;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigningKeyManagment.Models
{
    public class DNSNameModel : BindableBase
    {
        public string CASubjectOU { get; set; }
        public string CASubjectO { get; set; }
        public string CASubjectC { get; set; }
        public string CASubjectCN { get; set; }
        string subj_cn;
        public string SubjectCN
        {
            get { return subj_cn; }
            set
            {
                SetProperty(ref subj_cn, value);
            }
        }
        string subj_ou;
        public string SubjectOU
        {
            get { return subj_ou; }
            set
            {
                SetProperty(ref subj_ou, value);
            }
        }
        string subj_o;
        public string SubjectO
        {
            get { return subj_o; }
            set
            {
                SetProperty(ref subj_o, value);
            }
        }
        string subj_c;
        public string SubjectC
        {
            get { return subj_c; }
            set
            {
                SetProperty(ref subj_c, value);
            }
        }
        bool dns_enabled;
        public bool DNSEnabled
        {
            get { return dns_enabled; }
            set
            {
                SetProperty(ref dns_enabled, value);
            }
        }
        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(SubjectCN) || string.IsNullOrEmpty(SubjectOU) || string.IsNullOrEmpty(SubjectO)
                || string.IsNullOrEmpty(SubjectC))
                return true;
            else
                return false;
        }
        public void SetDefaultName(MANUFACTURER m ,CERTTYPE ct,ENVIROMENT e,STORETYPE st)
        {
            string env = Converter.Env(e);
            string manu = Converter.Manu(m);
            string cert = Converter.CertType(ct);
            string store = Converter.ST(st);
            if(m==MANUFACTURER.SIX)
            {
                DNSEnabled = false;
                SubjectOU = "ATM";
                SubjectO = "SIX";
                SubjectC = "CH";
                CASubjectOU = SubjectOU;
                CASubjectO = SubjectO;
                CASubjectC = SubjectC;
                if (ct==CERTTYPE.QA)
                {
                    SubjectCN = store + "_" + cert + "_SOFTWARE_SIGNING";
                }
                else
                {                    
                    SubjectCN = store + "_" + cert+"_"+env + "_SOFTWARE_SIGNING";
                    CASubjectCN= store + "_" + "CA" + "_" + env + "_SOFTWARE_SIGNING";
                }
            }
            else
            {
                DNSEnabled = true;
                SubjectCN = manu + "_" + env + "_SOFTWARE_SIGNING";
                SubjectOU = "ATM";
                SubjectO = manu;
                SubjectC = "CH";
            }
        }
    }
}
