/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Mvc;
using SimulasiAPBN.Application;
using IValidatorFactory = SimulasiAPBN.Web.Validation.IValidatorFactory;

namespace SimulasiAPBN.Web.Controllers.Engine.Common
{
    [ApiController]
    [Produces("application/json")]
    [Route("Engine/[controller]")]
    public class EngineController : ControllerBase
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IValidatorFactory Validator;

        public EngineController(IUnitOfWork unitOfWork, IValidatorFactory validator)
        {
            UnitOfWork = unitOfWork;
            Validator = validator;
        }
    }
}