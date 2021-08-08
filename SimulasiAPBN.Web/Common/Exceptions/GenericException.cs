/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Net;
using SimulasiAPBN.Web.Models.Engine;

namespace SimulasiAPBN.Web.Common.Exceptions
{
    public class GenericException : Exception
    {
        public GenericException() 
        {
            Code = EngineErrorCode.GenericServerError;
            Status = HttpStatusCode.InternalServerError;
        }
        
        public GenericException(string message) : base(message)
        {
            Code = EngineErrorCode.GenericServerError;
            Status = HttpStatusCode.InternalServerError;
        }
        
        public GenericException(string message, EngineErrorCode code) : base(message)
        {
            Code = code;
            Status = HttpStatusCode.InternalServerError;
        }
        
        public EngineErrorCode Code { get; }
        public HttpStatusCode Status { get; protected set; }
    }
}