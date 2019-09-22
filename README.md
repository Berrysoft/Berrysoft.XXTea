# Berrysoft.XXTea
A .NET implementation of [TEA](https://en.wikipedia.org/wiki/Tiny_Encryption_Algorithm), [XTEA](https://en.wikipedia.org/wiki/XTEA) and [XXTEA](https://en.wikipedia.org/wiki/XXTEA) algorithm.

## Usage
``` csharp
var cryptor = new XXTeaCryptor("SuperStrongKey");
var encryptedData = cryptor.EncryptString("Hello world!"); // Encrypt
var decryptedData = cryptor.DecryptString(encryptedData); // Decrypt
```
