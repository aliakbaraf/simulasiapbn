/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Web.Common.Diagnostics;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Controllers
{
	[Controller]
	[Route("[controller]")]
	[AllowAnonymous]
	public class ErrorController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;
		
		public ErrorController(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}
		
		private void PopulateExceptionInformation(Exception exception = null)
		{
			ViewBag.GoogleTagManagerId = GoogleTagManager.GetId(_configuration);
			ViewBag.SupportId = null;
			ViewBag.ErrorMessage = exception?.Message;
		}

		[HttpGet]
		[Route("AccessDenied")]
		public IActionResult AccessDeniedError()
		{
			try
			{
				PopulateExceptionInformation();
				return View();
			}
			catch (Exception exception)
			{
				PopulateExceptionInformation(exception);
				return View();
			}
		}
		
		[HttpGet]
		[Route("Compatibility")]
		public IActionResult CompatibilityError()
		{
			try
			{
				PopulateExceptionInformation();
				return View();
			}
			catch (Exception exception)
			{
				PopulateExceptionInformation(exception);
				return View();
			}
		}
		
		[HttpGet]
		[Route("")]
		public IActionResult DefaultError([FromQuery] bool engine)
		{
			ViewBag.IsDevelopment = _env.IsDevelopment();
			try
			{
				PopulateExceptionInformation();

				var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
				if (exceptionHandlerFeature is null || exceptionHandlerFeature?.Exception is null)
				{
					return Redirect("/");
				}

				PopulateExceptionInformation(exceptionHandlerFeature.Exception);
				ViewBag.SupportId = exceptionHandlerFeature.SupportId;

				return View();
			}
			catch (Exception exception)
			{
				PopulateExceptionInformation(exception);
				return View();
			}
		}

		[HttpGet]
		[Route("Trigger")]
		public IActionResult TriggerError()
		{
			if (_env.IsDevelopment())
			{
				throw new ServiceUnavailableException("Sebuah kesalahan berhasil dipicu.");
			}

			return Redirect("/");
		}
	}
}