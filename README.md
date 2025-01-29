# Certificate Chain Dumper

A .NET console application that inspects and displays SSL/TLS certificate chains for any HTTPS URL.

The tool accepts any certificate (including invalid ones) while showing detailed information about the certificates and their validation status.

## Features

- Supports both GET and POST HTTP methods
- Displays complete certificate chain information
- Shows certificate validation status and SSL policy errors
- Prints detailed certificate properties including:
    - Hash and thumbprint
    - Subject and issuer
    - Validity period
    - Serial number
    - Public key details
    - Certificate extensions

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download) or later

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/marceloduplicati/certchaindump.git
cd certchaindump
```

2. Build the project:
```bash
dotnet build
```

## Running the Application

### GET Request
```bash
dotnet run GET https://example.com
```

### POST Request with JSON Body
```bash
dotnet run POST https://api.example.com {"key": "value"}
```

## Example Output

```
Certificate validation: True

SSL Policy errors: None

=== Certificate Information ===
Certificate Hash: A1B2C3...
Subject: CN=example.com
Issuer: CN=DigiCert Global Root CA
Valid From: 2023-01-01
Valid To: 2024-01-01
...

=== Certificate Chain ===
Chain Element Certificate:
Certificate Hash: X1Y2Z3...
Subject: CN=DigiCert Global Root CA
...
```

## Notes

- The application always accepts certificates regardless of their validity
- Response bodies are truncated to the first 64 bytes in the output
- All certificate validation errors and SSL policy issues are displayed for inspection

## Error Handling

The application handles various error scenarios:
- Invalid URLs
- Network connectivity issues
- Certificate parsing errors
- Invalid HTTP methods

## License

This project is open source and available under the MIT License.