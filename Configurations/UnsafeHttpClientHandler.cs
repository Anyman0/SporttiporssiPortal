namespace SporttiporssiPortal.Configurations
{
    public class UnsafeHttpClientHandler : HttpClientHandler
    {
        public UnsafeHttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }
    }
}
