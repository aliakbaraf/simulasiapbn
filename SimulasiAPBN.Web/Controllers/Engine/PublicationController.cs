using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Controllers.Engine.Common;
using SimulasiAPBN.Web.Models.Engine;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Controllers.Engine
{
	public class PublicationController : EngineController
	{
		public PublicationController(IUnitOfWork unitOfWork, IValidatorFactory validatorFactory) 
			: base(unitOfWork, validatorFactory) { }

		[HttpPost]
		[Route("share")]
		public async Task<EngineResponse> CreateSimulationShare(
			[FromForm] string sessionId,
			[FromForm] string target)
		{
			UnitOfWork.BeginTransaction();
			try
			{
				if (string.IsNullOrEmpty(sessionId))
				{
					throw new BadRequestException("Identitas sesi diperlukan, namun tidak ada dalam parameter.",
						EngineErrorCode.RequiredDataNotProvided);
				}
				
				if (string.IsNullOrEmpty(target))
				{
					throw new BadRequestException("Target publikasi diperlukan, namun tidak ada dalam parameter.",
						EngineErrorCode.RequiredDataNotProvided);
				}

				if (!Guid.TryParse(sessionId, out var sessionGuid))
				{
					throw new BadRequestException("Format identitas sesi tidak valid.",
						EngineErrorCode.InvalidDataFormat);
				}

				if (target is not "facebook" && target is not "twitter")
				{
					throw new BadRequestException("Format target publikasi tidak valid.",
						EngineErrorCode.InvalidDataFormat);
				}

				var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
				if (session is null)
				{
					throw new NotFoundException("Data sesi tersebut tidak ditemukan.",
						EngineErrorCode.DataNotFound);
				}

				var shareTarget = target switch
				{
					"facebook" => SimulationShareTarget.FacebookPost,
					"twitter" => SimulationShareTarget.TwitterTweet,
					_ => SimulationShareTarget.Unknown
				};
				var share = new SimulationShare
				{
					SimulationSession = session,
					Target = shareTarget,
					ClickedTimes = 0
				};
				await UnitOfWork.SimulationShares.AddAsync(share);
				
				await UnitOfWork.CommitAsync();

				return new EngineResponse(share);
			}
			catch (Exception)
			{
				await UnitOfWork.RollbackAsync();
				throw;
			}
		}
	}
}