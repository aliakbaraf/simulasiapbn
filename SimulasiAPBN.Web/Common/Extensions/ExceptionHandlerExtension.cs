﻿/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Builder;
using SimulasiAPBN.Web.Common.Middlewares;

namespace SimulasiAPBN.Web.Common.Extensions
{

    public static class ExceptionHandlerExtension
    {
        public static IApplicationBuilder UseExceptionPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
