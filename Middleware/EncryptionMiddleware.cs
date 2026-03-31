using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace OTC.Api.Middleware;

public class EncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        var encryptionSettings = configuration.GetSection("Encryption");
        _key = Encoding.UTF8.GetBytes(encryptionSettings["Key"] ?? throw new ArgumentNullException("Encryption:Key"));
        _iv = Encoding.UTF8.GetBytes(encryptionSettings["IV"] ?? throw new ArgumentNullException("Encryption:IV"));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only process POST requests for decryption
        if (context.Request.Method == HttpMethods.Post && context.Request.ContentType?.Contains("application/json") == true)
        {
            await DecryptRequest(context);
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try 
        {
            await _next(context);
        }
        finally 
        {
            // Only process POST requests for response encryption
            if (context.Request.Method == HttpMethods.Post && context.Response.ContentType?.Contains("application/json") == true)
            {
                await EncryptResponse(context, responseBody, originalBodyStream);
            }
            else
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

    private async Task DecryptRequest(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        
        try 
        {
            if (!string.IsNullOrEmpty(body))
            {
                var wrappedRequest = JsonSerializer.Deserialize<JsonElement>(body);
                if (wrappedRequest.ValueKind == JsonValueKind.Object && wrappedRequest.TryGetProperty("encryptedata", out var encryptedData))
                {
                    var encryptedString = encryptedData.GetString();
                    if (!string.IsNullOrEmpty(encryptedString))
                    {
                        var decryptedContent = DecryptString(encryptedString);
                        var requestData = Encoding.UTF8.GetBytes(decryptedContent);
                        var stream = new MemoryStream(requestData);
                        context.Request.Body = stream;
                        context.Request.ContentLength = requestData.Length;
                    }
                }
            }
        }
        catch (Exception)
        {
            // If decryption fails, we leave the body as is (or could throw 400)
            context.Request.Body.Position = 0;
        }
    }

    private async Task EncryptResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseBody).ReadToEndAsync();
        
        if (!string.IsNullOrEmpty(body))
        {
            try 
            {
                var encrypted = EncryptString(body);
                var wrappedResponse = JsonSerializer.Serialize(new { encryptedata = encrypted });
                
                var responseData = Encoding.UTF8.GetBytes(wrappedResponse);
                context.Response.ContentLength = responseData.Length;
                await originalBodyStream.WriteAsync(responseData, 0, responseData.Length);
            }
            catch (Exception)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        else 
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private string EncryptString(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    private string DecryptString(string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}
