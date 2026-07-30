using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using San_Pham_Do_An1.Models;
using San_Pham_Do_An1.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace San_Pham_Do_An1.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;

        public VnPayService(IOptions<VnPaySettings> options)
        {
            _settings = options.Value;
        }

        public string CreatePaymentUrl(HttpContext httpContext, VnPayRequest request)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var createDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
            // VNPAY yêu cầu sort key theo thứ tự ASCII (Ordinal)
            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = _settings.TmnCode,
                ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(CultureInfo.InvariantCulture),
                ["vnp_CreateDate"] = createDate.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = GetIpAddress(httpContext),
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = request.OrderDescription,
                ["vnp_OrderType"] = "other",
                ["vnp_ReturnUrl"] = GetReturnUrl(httpContext),
                ["vnp_TxnRef"] = request.OrderId,
                ["vnp_ExpireDate"] = createDate.AddMinutes(15).ToString("yyyyMMddHHmmss")
            };

            // Nếu cấu hình BankCode (ví dụ: "NCB") thì gửi vnp_BankCode cố định để test.
            // Nếu để rỗng, VNPAY sẽ hiển thị màn hình chọn ngân hàng.
            if (!string.IsNullOrWhiteSpace(_settings.BankCode))
            {
                vnpParams["vnp_BankCode"] = _settings.BankCode;
            }

            // VNPAY yêu cầu chuỗi ký sử dụng giá trị đã UrlEncode, key sort tăng dần, nối key=value bằng &
            var signData = BuildDataToSign(vnpParams);
            var vnpSecureHash = HmacSHA512(_settings.HashSecret, signData);

            var queryString = BuildDataToSign(vnpParams);

            // VNPAY 2.1.0 không cần gửi vnp_SecureHashType nữa, gửi đi sẽ gây lỗi sai chữ ký trên cổng VNPAY
            var url = $"{_settings.PaymentUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";

            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("=== VNPAY CREATE PAYMENT URL DEBUG ===");
                sb.AppendLine($"Time: {DateTime.Now}");
                sb.AppendLine($"TmnCode: {_settings.TmnCode}");
                sb.AppendLine($"HashSecret: {_settings.HashSecret}");
                sb.AppendLine($"PaymentUrl base: {_settings.PaymentUrl}");
                sb.AppendLine($"Generated URL: {url}");
                sb.AppendLine($"Sign Data: {signData}");
                sb.AppendLine($"vnp_SecureHash: {vnpSecureHash}");
                sb.AppendLine("Parameters:");
                foreach (var kvp in vnpParams)
                {
                    sb.AppendLine($"  {kvp.Key} = {kvp.Value}");
                }
                sb.AppendLine("======================================");
                System.IO.File.AppendAllText("vnpay_debug.txt", sb.ToString());
            }
            catch {}

            return url;
        }

        public bool ValidateSignature(IQueryCollection query)
        {
            var receivedHash = query["vnp_SecureHash"].ToString();
            if (string.IsNullOrEmpty(receivedHash))
            {
                return false;
            }

            var sortedParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var param in query)
            {
                if (!param.Key.StartsWith("vnp_"))
                {
                    continue;
                }

                if (param.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) ||
                    param.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                sortedParams[param.Key] = param.Value.ToString();
            }

            // Chuỗi gốc để ký lại phải encode value giống cách VNPAY tạo chữ ký
            var signData = BuildDataToSign(sortedParams);
            var calculatedHash = HmacSHA512(_settings.HashSecret, signData);

            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("=== VNPAY VALIDATE SIGNATURE DEBUG ===");
                sb.AppendLine($"Time: {DateTime.Now}");
                sb.AppendLine($"HashSecret: {_settings.HashSecret}");
                sb.AppendLine($"Received Hash: {receivedHash}");
                sb.AppendLine($"Calculated Hash: {calculatedHash}");
                sb.AppendLine($"Sign Data: {signData}");
                sb.AppendLine("Parameters:");
                foreach (var kvp in sortedParams)
                {
                    sb.AppendLine($"  {kvp.Key} = {kvp.Value}");
                }
                sb.AppendLine("======================================");
                System.IO.File.AppendAllText("vnpay_debug.txt", sb.ToString());
            }
            catch {}

            return string.Equals(receivedHash, calculatedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Xây dựng chuỗi dữ liệu theo chuẩn VNPAY:
        /// key1=value1&key2=value2... với key đã sort tăng dần.
        /// </summary>
        private string BuildDataToSign(SortedDictionary<string, string> vnpParams)
        {
            var builder = new StringBuilder();
            foreach (var kvp in vnpParams)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(kvp.Key);
                builder.Append('=');
                builder.Append(WebUtility.UrlEncode(kvp.Value));
            }

            return builder.ToString();
        }

        private static string GetIpAddress(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }
            return ip;
        }

        private string GetReturnUrl(HttpContext context)
        {
            if (!string.IsNullOrWhiteSpace(_settings.ReturnUrl))
            {
                return _settings.ReturnUrl;
            }

            return $"{context.Request.Scheme}://{context.Request.Host}/Checkout/PaymentCallback";
        }

        private static string HmacSHA512(string key, string inputData)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key ?? string.Empty));
            var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData ?? string.Empty));
            return BitConverter.ToString(hashValue).Replace("-", string.Empty);
        }
    }
}

