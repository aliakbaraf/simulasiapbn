/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Core.Extensions
{
	public static class SignInSessionExtension
	{
		private static readonly TimeSpan SignInSessionExpirationTime = TimeSpan.FromHours(24);

		public static bool HasBeenRevoked(this SignInSession signInSession)
		{
			var nowTime = DateTimeOffset.Now.TimeOfDay;
			var lastActivityTime = (signInSession.UpdatedAt ?? signInSession.CreatedAt).TimeOfDay;
			var revokeTime = lastActivityTime.Add(SignInSessionExpirationTime);
			
			var isTimeRevoked = lastActivityTime > nowTime || nowTime >= revokeTime;
			
			return isTimeRevoked || signInSession.IsRevoked;
		}

		public static Guid ToSessionId(this IEnumerable<Claim> claims)
		{
			var sidClaim = claims.ToList()
				.Find(claim => claim.Type == ClaimTypes.Sid);
			
			return sidClaim is null ? Guid.Empty : Guid.Parse(sidClaim.Value);
		}
	}
}