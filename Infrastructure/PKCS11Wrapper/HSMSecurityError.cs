namespace Infrastructure.Pkcs11wrapper
{
    public class HSMSecurityError
    {
        public int Code
        {
            get;
            set;
        }
        public int RVCode
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public DateTime Occurred
        {
            get;
            set;
        }
    }
}
