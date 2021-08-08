/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Threading.Tasks;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ISimulationConfigRepository : ISimulationConfigRepository<IRepositoryOptions> {}
    
    public interface ISimulationConfigRepository<out TRepositoryOptions> 
        : IGenericRepository<SimulationConfig, TRepositoryOptions>
        where TRepositoryOptions : IRepositoryOptions
    {
        Task<SimulationConfig?> GetByKeyAsync(SimulationConfigKey key);

        Task<bool> IsAppInstalledAsync();

        Task SetAppInstalledAsync(bool isAppInstalled);

        Task<long?> GetLastSetupTimeAsync();

        Task SetLastSetupTimeAsync(long lastSetupTime);

        Task<decimal> GetDeficitThresholdAsync();

        Task SetDeficitThresholdAsync(decimal deficitThreshold);

        Task<string> GetDeficitLawAsync();

        Task SetDeficitLawAsync(string deficitLaw);
        
        Task<decimal> GetDebtRatioAsync();

        Task SetDebtRatioAsync(decimal debtRatio);

        Task<decimal> GetGrossDomesticProductAsync();

        Task SetGrossDomesticProductAsync(decimal deficitThreshold);


    }
}