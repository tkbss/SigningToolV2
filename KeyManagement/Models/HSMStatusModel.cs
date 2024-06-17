using Infrastructure.HSM;
using Prism.Mvvm;


namespace SigningKeyManagment.Models
{
    
    public class HSMStatusModel : BindableBase
    {
        string ip_adr;
        public string IPAdr
        {
            get { return ip_adr; }
            set
            {
                SetProperty(ref ip_adr, value);
            }
        }
        string connection;
        public string Connection
        {
            get
            {
                return connection;
            }
            set
            {
                SetProperty(ref connection, value);
            }
        }
        string slot;
        public string Slot
        {
            get
            {
                return slot;
            }
            set
            {
                SetProperty(ref slot, value);
            }
        }


        

        public HSMStatusModel() { }
        public HSMStatusModel(HSMStatusInfo s)
        {
            IPAdr = s.IPAdr;
            Connection = s.Connection;
            if (s.Slot < 0)
                Slot = "MISSING";
            else
                Slot = Convert.ToString(s.Slot);
        }
    }
}
