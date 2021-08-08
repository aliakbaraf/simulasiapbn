/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Data;
using System.Threading.Tasks;
using SimulasiAPBN.Application.Repositories;

namespace SimulasiAPBN.Application
{
    public interface IUnitOfWork : IUnitOfWork<IRepositoryOptions> {}
    
    public interface IUnitOfWork<out TRepositoryOptions> : IDisposable, IAsyncDisposable
        where TRepositoryOptions : IRepositoryOptions
    {
        IAccountRepository<TRepositoryOptions> Accounts { get; }
        IAllocationRepository<TRepositoryOptions> Allocations { get; }
        IBudgetRepository<TRepositoryOptions> Budgets { get; }
        IBudgetTargetRepository<TRepositoryOptions> BudgetTargets { get; }
        IEconomicMacroRepository<TRepositoryOptions> EconomicMacros { get; }
        IMediaFileRepository<TRepositoryOptions> MediaFiles { get; }
        ISignInAttemptRepository<TRepositoryOptions> SignInAttempts { get; }
        ISignInSessionRepository<TRepositoryOptions> SignInSessions { get; }
        ISimulationConfigRepository<TRepositoryOptions> SimulationConfigs { get; }
        ISimulationEconomicMacroRepository<TRepositoryOptions> SimulationEconomicMacros { get; }
        ISimulationSessionRepository<TRepositoryOptions> SimulationSessions { get; }
        ISimulationShareRepository<TRepositoryOptions> SimulationShares { get; }
        ISimulationSpecialPolicyAllocationRepository<TRepositoryOptions> SimulationSpecialPolicyAllocations { get; }
        ISimulationStateExpenditureRepository<TRepositoryOptions> SimulationStateExpenditures { get; }
        ISpecialPolicyRepository<TRepositoryOptions> SpecialPolicies { get; }
        ISpecialPolicyAllocationRepository<TRepositoryOptions> SpecialPolicyAllocations { get; }
        IStateBudgetRepository<TRepositoryOptions> StateBudgets { get; }
        IStateExpenditureRepository<TRepositoryOptions> StateExpenditures { get; }
        IStateExpenditureAllocationRepository<TRepositoryOptions> StateExpenditureAllocations { get; }
        ITokenRepository<TRepositoryOptions> Tokens { get; }
        IWebContentRepository<TRepositoryOptions> WebContents { get; }
        
        IDbConnection DbConnection { get; }
        
        IDbTransaction? DbTransaction { get; }
        
        bool IsUsingTransaction { get; }

        void BeginTransaction();

        void Commit();

        Task CommitAsync();

        void Rollback();

        Task RollbackAsync();

        void UseProvider(int provider);
    }
}