namespace Infrastructure.Pkcs11wrapper
{
    public class HSMDriverException : Exception
    {
        public HSMSecurityError DriverError
        {
            get;
            set;
        }
    }
}
