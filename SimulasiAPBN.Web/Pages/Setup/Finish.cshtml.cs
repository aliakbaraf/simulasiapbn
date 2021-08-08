/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.CompilerServices;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Extensions;
using SimulasiAPBN.Web.Models;

namespace SimulasiAPBN.Web.Pages.Setup
{
    public class Finish : BasePage
    {
        private const long MaxAge = 60 * 1000;
        private readonly IMemoryCache _memoryCache;
        private readonly IUnitOfWork _unitOfWork;

        public Finish(
            IConfiguration configuration,
            IMemoryCache memoryCache, 
            IUnitOfWork unitOfWork)
            : base(configuration)
        {
            _memoryCache = memoryCache;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IActionResult> OnGet()
        {
            var isAppInstalledConfig = await _unitOfWork.SimulationConfigs
                .GetByKeyAsync(SimulationConfigKey.IsAppInstalled);
            if (isAppInstalledConfig is null)
            {
                return Redirect("/setup");
            }

            if (isAppInstalledConfig.Value != true.ToString())
            {
                return Redirect("/setup");
            }
            
            var lastAppSetupRunConfig = await _unitOfWork.SimulationConfigs
                .GetByKeyAsync(SimulationConfigKey.LastAppSetupRun);
            if (lastAppSetupRunConfig is null)
            {
                return Redirect("/setup");
            }
            var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var lastAppSetupRunTime = LongType.FromString(lastAppSetupRunConfig.Value);
            var age = currentTime - lastAppSetupRunTime;
            
            if (age > MaxAge)
            {
                return Redirect("/");
            }

            if (!_memoryCache.TryGetValue(CacheKeys.IsAppInstalled, out bool isAppInstalled))
            {
                return Page();
            }
            
            isAppInstalled = isAppInstalledConfig.Value == true.ToString();

            _memoryCache.SetIsAppInstalled(isAppInstalled);

            return Page();
        }
    }
}