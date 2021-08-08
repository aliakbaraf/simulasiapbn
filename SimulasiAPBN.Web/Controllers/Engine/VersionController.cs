/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SimulasiAPBN.Application;
using SimulasiAPBN.Web.Controllers.Engine.Common;
using SimulasiAPBN.Web.Models.Engine;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Controllers.Engine
{
    public class VersionController : EngineController
    {
        public VersionController(IUnitOfWork unitOfWork, IValidatorFactory validatorFactory) 
            : base(unitOfWork, validatorFactory)
        {
        }

        [HttpGet("")]
        public EngineResponse GetVersion()
        {
            var version = new Dictionary<string, System.Version>
            {
                { "engine", GetType().Assembly.GetName().Version },
                { "runtime", System.Environment.Version }
            };  
            return new EngineResponse(version);
        }
    }
}
