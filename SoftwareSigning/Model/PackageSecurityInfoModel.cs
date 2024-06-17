using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoftwareSigning.Model
{
    public class PackageSecurityInfoModel : BindableBase
    {
        Brush md_status;
        public Brush MDStatus
        {
            get { return md_status; }
            set
            {
                SetProperty(ref md_status, value);
            }
        }
        string digest;
        public string Digest
        {
            get { return digest; }
            set
            {
                SetProperty(ref digest, value);
            }
        }
        string comp_digest;
        public string ComputedDigest
        {
            get { return comp_digest; }
            set
            {
                SetProperty(ref comp_digest, value);
            }
        }
        string file_name;
        public string FileName
        {
            get { return file_name; }
            set
            {
                SetProperty(ref file_name, value);
            }
        }
        string algorithm;
        public string Algorithm
        {
            get { return algorithm; }
            set
            {
                SetProperty(ref algorithm, value);
            }
        }
    }
}
