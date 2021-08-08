/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Common.Middlewares
{
    public class EngineRoutingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public EngineRoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/engine"))
            {
                throw new NotFoundException();
            }

            await _next(context);
        }
    }
}