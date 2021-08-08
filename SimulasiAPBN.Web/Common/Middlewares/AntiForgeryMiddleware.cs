/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Common.Middlewares
{
    public class AntiForgeryMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AntiForgeryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAntiforgery antiForgery)
        {
            if (await antiForgery.IsRequestValidAsync(context))
            {
                throw new BadRequestException(
                    "Permintaan Anda gagal karena sesi CSRF telah berakhir. Silakan coba lagi.");
            }

            await _next(context);
        }
    }
}