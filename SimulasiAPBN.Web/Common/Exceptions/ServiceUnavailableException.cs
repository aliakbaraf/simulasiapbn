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
    public class ServiceUnavailableException : GenericException
    {
        public ServiceUnavailableException() : base("Service Unavailable")
        {
            Status = HttpStatusCode.ServiceUnavailable;
        }

        public ServiceUnavailableException(string message) : base(message)
        {
            Status = HttpStatusCode.ServiceUnavailable;
        }

        public ServiceUnavailableException(string message, EngineErrorCode code) : base(message, code)
        {
            Status = HttpStatusCode.ServiceUnavailable;
        }
    }
}