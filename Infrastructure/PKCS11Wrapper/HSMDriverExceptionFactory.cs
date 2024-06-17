namespace Infrastructure.Pkcs11wrapper
{
    public class HSMDriverExceptionFactory
    {
        public static HSMDriverException Create(int rv,int code) 
        {
            HSMSecurityError error = new HSMSecurityError();
            error.RVCode = rv;
            error.Code = code;
            error.Description = HSMDriverErrorDefinitions.Description(code);
            error.Occurred = DateTime.Now;
            return new HSMDriverException() { DriverError = error };
        }
        public static HSMDriverException Create(int rv, int code,string parameter)
        {
            HSMSecurityError error = new HSMSecurityError();
            error.RVCode = rv;
            error.Code = code;
            error.Description = HSMDriverErrorDefinitions.Description(code)+" "+parameter;
            error.Occurred = DateTime.Now;
            return new HSMDriverException() { DriverError = error };
        }
    }
}
