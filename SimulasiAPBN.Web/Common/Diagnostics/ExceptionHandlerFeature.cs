/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Http;
using SimulasiAPBN.Web.Models.Engine;
using System;
using System.Net;

namespace SimulasiAPBN.Web.Common.Diagnostics
{
    public class ExceptionHandlerFeature : IExceptionHandlerFeature
    {
        public ExceptionHandlerFeature(
            PathString path,
            Exception exception, 
            EngineErrorCode code,
            HttpStatusCode status, 
            string supportId)
        {
            Path = path;
            Exception = exception;
            Code = code;
            Status = status;
            SupportId = supportId;
        }

        public PathString Path { get; }
        public Exception Exception { get; }

        public EngineErrorCode Code { get; }
        public HttpStatusCode Status { get; }
        public string SupportId { get; }

    }
}
