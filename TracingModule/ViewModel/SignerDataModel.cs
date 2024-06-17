using Prism.Mvvm;
using System.Data;

namespace TracingModule.ViewModel
{
    public class SignerDataModel : BindableBase
    {
        string signer;
        public string Signer
        {
            get
            {
                return signer;
            }
            set
            {
                SetProperty(ref signer, value);
            }
        }
        string env;
        public string Enviroment
        {
            get
            {
                return env;
            }
            set
            {
                SetProperty(ref env, value);
            }
        }
        string signer_type;
        public string SignerType
        {
            get
            {
                return signer_type;
            }
            set
            {
                SetProperty(ref signer_type, value);
            }
        }
        string key_storage;
        public string KeyStorage
        {
            get
            {
                return key_storage;
            }
            set
            {
                SetProperty(ref key_storage, value);
            }
        }
        public void SetData(DataRowView Item)
        {
            Signer= (string)Item.Row["Signer"];
            SignerType= (string)Item.Row["Signer_Type"];
            KeyStorage= (string)Item.Row["StoreType"];
            Enviroment= (string)Item.Row["Enviroment"];
        }
    }
}
