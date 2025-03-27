using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication2.Payments;

public class VNPayHelper
{
    private readonly IConfiguration _configuration;

    public VNPayHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreatePaymentUrl(HttpContext httpContext, decimal amount, string orderId)
    {
        var tmnCode = _configuration["VNPaySettings:TmnCode"];
        var secretKey = _configuration["VNPaySettings:HashSecret"];
        var vnpUrl = _configuration["VNPaySettings:Url"];
        var returnUrl = _configuration["VNPaySettings:ReturnUrl"];

        var payParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Amount", (amount * 100).ToString() }, // VNPay requires amount in VND * 100
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", $"Payment for Order {orderId}" },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", httpContext.Connection.RemoteIpAddress?.ToString() ?? "172.17.144.1" },
            { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") }
        };

        // Generate secure hash
        string signData = string.Join("&", payParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        string hash = ComputeHmacSha512(secretKey, signData);

        // Append the secure hash
        payParams.Add("vnp_SecureHash", hash);

        // Construct final URL
        string paymentUrl = QueryHelpers.AddQueryString(vnpUrl, payParams);
        return paymentUrl;
    }
    public static string ComputeHmacSha512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
    }
}
