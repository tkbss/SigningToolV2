namespace Infrastructure.Exceptions
{
    public class SignatureVerificationException: Exception
    {
        public SignatureVerificationException() :base ()
        {

        }
        public SignatureVerificationException(string m) : base(m) { }
    }
    public class CertificateValidationException : Exception
    {
        public CertificateValidationException() : base()
        {

        }
        public CertificateValidationException(string m) : base(m) { }
    }
}
