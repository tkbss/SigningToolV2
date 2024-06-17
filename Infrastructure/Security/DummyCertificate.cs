
namespace Infrastructure
{
    class DummyCertificate
    {
        private static string cert =
        "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";

        public static string Get()
        {
            return cert;
        }
    }
}
