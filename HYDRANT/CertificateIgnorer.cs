using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HYDRANT;
internal class CertificateIgnorer : HttpClientHandler
{
    public CertificateIgnorer()
    {
        ServerCertificateCustomValidationCallback = CustomServerCertificateValidation;
    }

    private bool CustomServerCertificateValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
    {
        // WARNING: This bypasses all SSL certificate validation checks.
        // In production, implement proper certificate checks for security.
        return true; // Bypass all certificate validation checks
    }
}
