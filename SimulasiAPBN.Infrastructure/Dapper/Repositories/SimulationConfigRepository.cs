/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Globalization;
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.Dapper.Repositories
{
    public class SimulationConfigRepository 
        : GenericRepository<SimulationConfig>, ISimulationConfigRepository<RepositoryOptions>
    {
        public SimulationConfigRepository(RepositoryOptions options) : base(options) {}

        public Task<SimulationConfig> GetByKeyAsync(SimulationConfigKey key)
        {
            return FindOneAsync(appConfig => appConfig.Key == key);
        }

        public async Task<bool> IsAppInstalledAsync()
        {
            var isAppInstalledConfig = await GetByKeyAsync(SimulationConfigKey.IsAppInstalled);
            if (isAppInstalledConfig is null) return false;
            return isAppInstalledConfig.Value == "true";
        }

        public async Task SetAppInstalledAsync(bool isAppInstalled)
        {
            var isAppInstalledConfig = await GetByKeyAsync(SimulationConfigKey.IsAppInstalled);
            if (isAppInstalledConfig is null)
            {
                isAppInstalledConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.IsAppInstalled, 
                    Value = isAppInstalled.ToString()
                };
                await AddAsync(isAppInstalledConfig);
            }
            else
            {
                isAppInstalledConfig.Value = isAppInstalled.ToString();
                await ModifyAsync(isAppInstalledConfig);
            }
        }

        public async Task<long?> GetLastSetupTimeAsync()
        {
            var lastSetupRunConfig = await GetByKeyAsync(SimulationConfigKey.LastAppSetupRun);
            if (lastSetupRunConfig is null) return null;
            return Convert.ToInt64(lastSetupRunConfig.Value);
        }

        public async Task SetLastSetupTimeAsync(long lastSetupTime)
        {
            var lastSetupRunConfig = await GetByKeyAsync(SimulationConfigKey.LastAppSetupRun);
            if (lastSetupRunConfig is null)
            {
                lastSetupRunConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.LastAppSetupRun,
                    Value = lastSetupTime.ToString()
                };
                await AddAsync(lastSetupRunConfig);
            }
            else
            {
                lastSetupRunConfig.Value = lastSetupTime.ToString();
                await ModifyAsync(lastSetupRunConfig);
            }
        }

        public async Task<decimal> GetDeficitThresholdAsync()
        {
            var deficitThresholdConfig = await GetByKeyAsync(SimulationConfigKey.DeficitThreshold);
            if (deficitThresholdConfig is null) return decimal.One;
            var deficitThresholdString = deficitThresholdConfig.Value
                .Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.TryParse(deficitThresholdString, NumberStyles.Any, CultureInfo.InvariantCulture, 
                out var deficitThreshold)
                ? deficitThreshold
                : decimal.One;
        }

        public async Task SetDeficitThresholdAsync(decimal deficitThreshold)
        {
            var deficitThresholdConfig = await GetByKeyAsync(SimulationConfigKey.DeficitThreshold);
            if (deficitThresholdConfig is null)
            {
                deficitThresholdConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.DeficitThreshold,
                    Value = deficitThreshold.ToString(CultureInfo.InvariantCulture)
                };
                await AddAsync(deficitThresholdConfig);
            }
            else
            {
                deficitThresholdConfig.Value = deficitThreshold.ToString(CultureInfo.InvariantCulture);
                await ModifyAsync(deficitThresholdConfig);
            }
        }

        public async Task<string> GetDeficitLawAsync()
        {
            var deficitLawConfig = await GetByKeyAsync(SimulationConfigKey.DeficitLaw);
            return deficitLawConfig?.Value;
        }

        public async Task SetDeficitLawAsync(string deficitLaw)
        {
            var deficitLawConfig = await GetByKeyAsync(SimulationConfigKey.DeficitLaw);
            if (deficitLawConfig is null)
            {
                deficitLawConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.DeficitLaw,
                    Value = deficitLaw
                };
                await AddAsync(deficitLawConfig);
            }
            else
            {
                deficitLawConfig.Value = deficitLaw;
                await ModifyAsync(deficitLawConfig);
            }
        }

        public async Task<decimal> GetDebtRatioAsync()
        {
            var deficitThresholdConfig = await GetByKeyAsync(SimulationConfigKey.DebtRatio);
            if (deficitThresholdConfig is null) return decimal.One;
            var deficitThresholdString = deficitThresholdConfig.Value
                .Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.TryParse(deficitThresholdString, NumberStyles.Any, CultureInfo.InvariantCulture, 
                out var deficitThreshold)
                ? deficitThreshold
                : decimal.One;
        }

        public async Task SetDebtRatioAsync(decimal debtRatio)
        {
            var deficitLawConfig = await GetByKeyAsync(SimulationConfigKey.DebtRatio);
            if (deficitLawConfig is null)
            {
                deficitLawConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.DeficitLaw,
                    Value = debtRatio.ToString(CultureInfo.InvariantCulture)
                };
                await AddAsync(deficitLawConfig);
            }
            else
            {
                deficitLawConfig.Value = debtRatio.ToString(CultureInfo.InvariantCulture);
                await ModifyAsync(deficitLawConfig);
            }
        }

        public async Task<decimal> GetGrossDomesticProductAsync()
        {
            var grossDomesticProductConfig = await GetByKeyAsync(SimulationConfigKey.GrossDomesticProduct);
            if (grossDomesticProductConfig is null) return decimal.One;
            var grossDomesticProductString = grossDomesticProductConfig.Value
                .Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.TryParse(grossDomesticProductString, NumberStyles.Any, CultureInfo.InvariantCulture,
                out var grossDomesticProduct)
                ? grossDomesticProduct
                : decimal.Zero;
        }

        public async Task SetGrossDomesticProductAsync(decimal grossDomesticProduct)
        {
            var grossDomesticProductConfig = await GetByKeyAsync(SimulationConfigKey.GrossDomesticProduct);
            if (grossDomesticProductConfig is null)
            {
                grossDomesticProductConfig = new SimulationConfig
                {
                    Key = SimulationConfigKey.GrossDomesticProduct,
                    Value = grossDomesticProduct.ToString(CultureInfo.InvariantCulture)
                };
                await AddAsync(grossDomesticProductConfig);
            }
            else
            {
                grossDomesticProductConfig.Value = grossDomesticProduct.ToString(CultureInfo.InvariantCulture);
                await ModifyAsync(grossDomesticProductConfig);
            }
        }
    }
}