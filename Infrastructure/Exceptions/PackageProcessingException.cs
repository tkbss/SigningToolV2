namespace Infrastructure.Exceptions
{
    public class PackageProcessingException : Exception
    {
        public PackageProcessingException(string m) : base(m) { }
    }
}
