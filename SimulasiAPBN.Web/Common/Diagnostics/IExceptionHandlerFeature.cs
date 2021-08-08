/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using SimulasiAPBN.Web.Models.Engine;

namespace SimulasiAPBN.Web.Common.Diagnostics
{
    interface IExceptionHandlerFeature
    {
        public PathString Path { get; }
        public Exception Exception { get; }
        public EngineErrorCode Code { get; }
        public HttpStatusCode Status { get; }
        public string SupportId { get; }
    }
}
