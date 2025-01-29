using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace certchaindump;

static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: CertificateInspector <GET|POST> <url> [json-body-for-post]");
            return;
        }

        var method = args[0].ToUpper();
        var url = args[1];
        string? jsonBody = null;

        if (method == "POST" && args.Length > 2) jsonBody = string.Join(" ", args[2..]);

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = ValidateServerCertificate
        };

        using var client = new HttpClient(handler);
        
        try
        {
            HttpResponseMessage response;
            if (method == "GET")
            {
                response = await client.GetAsync(url);
            }
            else if (method == "POST")
            {
                var content = new StringContent(jsonBody ?? "", System.Text.Encoding.UTF8, "application/json");
                response = await client.PostAsync(url, content);
            }
            else
            {
                Console.WriteLine($"Unsupported method: {method}");
                return;
            }

            Console.WriteLine($"\nResponse Status: {response.StatusCode}");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Body (First 64 bytes): {responseBody[..Math.Min(responseBody.Length, 64)]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static bool ValidateServerCertificate(
        HttpRequestMessage request,
        X509Certificate2? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (certificate != null)
        {
            Console.WriteLine($"\nCertificate validation: {certificate.Verify()}");
            Console.WriteLine($"\nSSL Policy errors: {sslPolicyErrors.ToString()}");
            Console.WriteLine("\n=== Certificate Information ===");
            
            PrintCertificateDetails(certificate);

            Console.WriteLine("\n=== Certificate Chain ===");
            if (chain != null)
                foreach (var element in chain.ChainElements)
                {
                    Console.WriteLine("\nChain Element Certificate:");
                    PrintCertificateDetails(element.Certificate);

                    if (element.ChainElementStatus.Length > 0)
                    {
                        Console.WriteLine("\nChain Element Status:");
                        foreach (var status in element.ChainElementStatus)
                            Console.WriteLine($"- {status.StatusInformation}");
                    }
                }
            else
                Console.WriteLine("\nNo chain!");
        }
        // Always return true to accept any certificate so the operation succeeds
        return true;
    }

    private static void PrintCertificateDetails(X509Certificate2 cert)
    {
        Console.WriteLine($"Certificate Hash: {cert.GetCertHashString()}");
        Console.WriteLine($"Subject: {cert.Subject}");
        Console.WriteLine($"Issuer: {cert.Issuer}");
        Console.WriteLine($"Valid From: {cert.NotBefore}");
        Console.WriteLine($"Valid To: {cert.NotAfter}");
        Console.WriteLine($"Serial Number: {cert.SerialNumber}");
        Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
        Console.WriteLine($"Version: {cert.Version}");
        
        try
        {
            var key = cert.PublicKey;
            Console.WriteLine($"Public Key Algorithm: {key.Oid?.FriendlyName}");
            Console.WriteLine($"Public Key Parameters: {BitConverter.ToString(key.EncodedParameters.RawData)}");
        }
        catch
        {
            Console.WriteLine("Could not access public key details");
        }

        Console.WriteLine("\nExtensions:");
        foreach (var extension in cert.Extensions)
        {
            Console.WriteLine($"- {extension.Oid?.FriendlyName}: {extension.Oid?.Value}");
        }
    }
}