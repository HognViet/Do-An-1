using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Http;

namespace San_Pham_Do_An1.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext httpContext, VnPayRequest request);
        bool ValidateSignature(IQueryCollection query);
    }
}
