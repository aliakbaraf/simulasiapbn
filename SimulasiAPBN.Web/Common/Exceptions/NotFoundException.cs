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
    public class NotFoundException : GenericException
    {
        public NotFoundException() : base("Not Found")
        {
            Status = HttpStatusCode.NotFound;
        }

        public NotFoundException(string message) : base(message)
        {
            Status = HttpStatusCode.NotFound;
        }

        public NotFoundException(string message, EngineErrorCode code) : base(message, code)
        {
            Status = HttpStatusCode.NotFound;
        }
    }
}