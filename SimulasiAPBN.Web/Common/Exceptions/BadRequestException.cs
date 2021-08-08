/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Net;
using SimulasiAPBN.Web.Models.Engine;

namespace SimulasiAPBN.Web.Common.Exceptions
{
    public class BadRequestException : GenericException
    {
        public BadRequestException() : base("Bad Request")
        {
            Status = HttpStatusCode.BadRequest;
        }

        public BadRequestException(string message) : base(message)
        {
            Status = HttpStatusCode.BadRequest;
        }
        public BadRequestException(string message, EngineErrorCode code) : base(message, code) 
        {
            Status = HttpStatusCode.BadRequest;
        }
    }
}