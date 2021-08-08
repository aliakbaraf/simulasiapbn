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
	public class UnauthorizedException : GenericException
	{
		public UnauthorizedException() : base("Unauthorized")
		{
			Status = HttpStatusCode.Unauthorized;
		}

		public UnauthorizedException(string message) : base(message)
		{
			Status = HttpStatusCode.Unauthorized;
		}
		public UnauthorizedException(string message, EngineErrorCode code) : base(message, code) 
		{
			Status = HttpStatusCode.Unauthorized;
		}
	}
}