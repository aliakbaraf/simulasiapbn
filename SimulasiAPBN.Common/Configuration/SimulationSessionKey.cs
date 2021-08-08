using System;

namespace SimulasiAPBN.Common.Configuration
{
	public static class SimulationSessionKey
	{
		public static bool CookieHttpOnly { get; } = true;
		public static string CookieName { get; } = "SimulasiAPBN.SessionKey";
		public static DateTimeOffset CookieExpires { get; } = DateTimeOffset.Now.Add(TimeSpan.FromDays(30));
		public static bool CookieSecure { get; } = true;
		public static TimeSpan MaxAge { get; } = TimeSpan.FromDays(30);
	}
}