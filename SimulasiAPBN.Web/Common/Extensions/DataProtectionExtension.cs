/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SimulasiAPBN.Web.Common.Extensions
{
	public static class DataProtectionExtension
	{
		public static IDataProtectionBuilder AddDefaultDataProtection(this IServiceCollection services)
		{
			return services.AddDataProtection()
				.SetApplicationName(typeof(Startup).Assembly.GetName().Name ?? "SimulasiAPBN");
		}
	}
}