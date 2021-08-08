/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Infrastructure.EntityFrameworkCore.Repositories;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork<RepositoryOptions>
    {
        private readonly IUnitOfWork<IRepositoryOptions> _parent;
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;

        #region Constructor
        public UnitOfWork(IUnitOfWork<IRepositoryOptions> parent, ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            _parent = parent;
            _dbContext = dbContext;
            _disposed = false;

            DbConnection = null!;
            DbTransaction = null;
            
            var options = new RepositoryOptions(loggerFactory, _dbContext);
            Accounts = new AccountRepository(options);
            Allocations = new AllocationRepository(options);
            EconomicMacros = new EconomicMacroRepository(options);
            Budgets = new BudgetRepository(options);
            BudgetTargets = new BudgetTargetRepository(options);
            MediaFiles = new MediaFileRepository(options);
            SignInAttempts = new SignInAttemptRepository(options);
            SignInSessions = new SignInSessionRepository(options);
            SimulationConfigs = new SimulationConfigRepository(options);
            SimulationSessions = new SimulationSessionRepository(options);
            SimulationShares = new SimulationShareRepository(options);
            SimulationSpecialPolicyAllocations = new SimulationSpecialPolicyAllocationRepository(options);
            SimulationStateExpenditures = new SimulationStateExpenditureRepository(options);
            SimulationEconomicMacros = new SimulationEconomicMacroRepository(options);
            SpecialPolicies = new SpecialPolicyRepository(options);
            SpecialPolicyAllocations = new SpecialPolicyAllocationRepository(options);
            StateBudgets = new StateBudgetRepository(options);
            StateExpenditures = new StateExpenditureRepository(options);
            StateExpenditureAllocations = new StateExpenditureAllocationRepository(options);
            Tokens = new TokenRepository(options);
            WebContents = new WebContentRepository(options);
        }
        #endregion

        #region Helper Properties
        private IDbContextTransaction? DbContextTransaction => _dbContext.Database.CurrentTransaction;
        #endregion

        #region Repositories
        public IAccountRepository<RepositoryOptions> Accounts { get; }
        public IAllocationRepository<RepositoryOptions> Allocations { get; }
        public IEconomicMacroRepository<RepositoryOptions> EconomicMacros { get; }
        public IBudgetRepository<RepositoryOptions> Budgets { get; }
        public IBudgetTargetRepository<RepositoryOptions> BudgetTargets { get; }
        public IMediaFileRepository<RepositoryOptions> MediaFiles { get; }
        public ISignInAttemptRepository<RepositoryOptions> SignInAttempts { get; }
        public ISignInSessionRepository<RepositoryOptions> SignInSessions { get; }
        public ISimulationConfigRepository<RepositoryOptions> SimulationConfigs { get; }
        public ISimulationSessionRepository<RepositoryOptions> SimulationSessions { get; }
        public ISimulationShareRepository<RepositoryOptions> SimulationShares { get; }
        public ISimulationSpecialPolicyAllocationRepository<RepositoryOptions> SimulationSpecialPolicyAllocations { get; }
        public ISimulationStateExpenditureRepository<RepositoryOptions> SimulationStateExpenditures { get; }
        public ISimulationEconomicMacroRepository<RepositoryOptions> SimulationEconomicMacros { get; }
        public ISpecialPolicyRepository<RepositoryOptions> SpecialPolicies { get; }
        public ISpecialPolicyAllocationRepository<RepositoryOptions> SpecialPolicyAllocations { get; }
        public IStateBudgetRepository<RepositoryOptions> StateBudgets { get; }
        public IStateExpenditureRepository<RepositoryOptions> StateExpenditures { get; }
        public IStateExpenditureAllocationRepository<RepositoryOptions> StateExpenditureAllocations { get; }
        public ITokenRepository<RepositoryOptions> Tokens { get; }
        public IWebContentRepository<RepositoryOptions> WebContents { get; }
        #endregion

        #region IUnitOfWork Implementation
        public IDbConnection DbConnection { get; }
        
        public IDbTransaction? DbTransaction { get; }
        public bool IsUsingTransaction => DbTransaction is not null;

        public void BeginTransaction()
        {
            if (DbContextTransaction is null)
            {
                _dbContext.Database.BeginTransaction();
            }
        }
        
        public void Commit()
        {
            if (DbContextTransaction is null) return;
            
            _dbContext.Database.CurrentTransaction.Commit();
        }

        public async Task CommitAsync()
        {
            if (DbContextTransaction is null) return;
            
            await _dbContext.Database.CurrentTransaction.CommitAsync();
        }

        public void Rollback()
        {
            if (DbContextTransaction is null) return;
            
            _dbContext.Database.CurrentTransaction.Rollback();
        }

        public async Task RollbackAsync()
        {
            if (DbContextTransaction is null) return;
            
            await _dbContext.Database.CurrentTransaction.RollbackAsync();
        }

        public void UseProvider(int provider)
        {
            _parent.UseProvider(provider);
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _dbContext.Dispose();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            
            await _dbContext.DisposeAsync();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
        
    }
}