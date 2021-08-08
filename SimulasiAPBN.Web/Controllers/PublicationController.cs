/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Web.Controllers
{

    [Controller]
    [Route("[controller]")]
    public class PublicationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PublicationController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sessionId, string shareId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                ViewBag.GoogleTagManagerId = GoogleTagManager.GetId(_configuration);
            
                if (string.IsNullOrEmpty(sessionId) && string.IsNullOrEmpty(shareId))
                {
                    return Redirect("/");
                }

                var isSessionGiven = !string.IsNullOrEmpty(sessionId);
                var isShareGiven = !string.IsNullOrEmpty(shareId);

                var sessionGuid = Guid.Empty;
                var shareGuid = Guid.Empty;

                if (isSessionGiven && !Guid.TryParse(sessionId, out sessionGuid))
                {
                    return Redirect("/");
                }

                if (isShareGiven && !Guid.TryParse(shareId, out shareGuid))
                {
                    return Redirect("/");
                }

                SimulationSession session;
                if (isShareGiven)
                {
                    // SimulationShare, linked: SimulationSession
                    var share = await _unitOfWork.SimulationShares.GetByIdAsync(shareGuid);
                    if (share is null)
                    {
                        return Redirect("/");
                    }

                    // SimulationSession, linked: StateBudget
                    session = await _unitOfWork.SimulationSessions.GetByIdAsync(share.SimulationSessionId);

                    share.ClickedTimes += 1;
                    await _unitOfWork.SimulationShares.ModifyAsync(share);
                }
                else
                {
                    // SimulationSession, linked: StateBudget
                    session = await _unitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
                }

                if (session is null)
                {
                    return Redirect("/");
                }
                
                if (session.SimulationState != SimulationState.Completed)
                {
                    return Redirect("/game");
                }

                var simulationStateExpenditures = await _unitOfWork
                    .SimulationStateExpenditures
                    .GetBySimulationSessionAsync(session);

                //var totalIncome = session.StateBudget.CountryIncome;
                var totalIncome = session.UsedIncome;
                var totalExpenditure = simulationStateExpenditures
                    .Sum(expenditure => expenditure.TotalAllocation);

                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var titleWebContent = await _unitOfWork.WebContents.GetByKeyAsync(WebContentKey.Title);

                // (threshold * GDP) + income
                // expenditure <= income = surplus
                // expenditure > income = deficit
                // -- expenditure <= threshold = safe deficit
                // -- expenditure > threshold = illegal deficit
                ViewBag.BaseUrl = baseUrl;
                ViewBag.IsSurplus = totalIncome >= totalExpenditure;
                ViewBag.Session = session;
                ViewBag.Title = titleWebContent?.Value ?? "Simulasi ABPN";
                ViewBag.TotalExpenditure = totalExpenditure;
                ViewBag.TotalIncome = totalIncome;

                await _unitOfWork.CommitAsync();
                
                return View();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }

}
