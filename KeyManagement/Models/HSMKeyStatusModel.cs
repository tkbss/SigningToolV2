using Infrastructure.HSM;
using Prism.Mvvm;


namespace SigningKeyManagment.Models
{
    public class HSMKeyStatusModel : BindableBase
    {
        string key_name;
        public string KeyName
        {
            get
            {
                return key_name;
            }
            set
            {
                SetProperty(ref key_name, value);
            }
        }
        string key_type;
        public string KeyType
        {
            get
            {
                return key_type;
            }
            set
            {
                SetProperty(ref key_type, value);
            }
        }
        string key_status;
        public string KeyStatus
        {
            get
            {
                return key_status;
            }
            set
            {
                SetProperty(ref key_status, value);
            }
        }
        public HSMKeyStatusModel()
        {
        }
        public HSMKeyStatusModel(KEY_STATUS key)
        {
            KeyName=key.KEY_NAME;
            KeyType = key.TYPE;
            if (key.ESTABLISHED == true)
                KeyStatus = "ESTABLISHED";
            else
                KeyStatus = "MISSING";
        }
    }
}
