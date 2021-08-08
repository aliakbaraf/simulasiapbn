/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Infrastructure.Common;
using SimulasiAPBN.Infrastructure.EntityFrameworkCore;

namespace SimulasiAPBN.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbService _dbService;
        private IUnitOfWork<IRepositoryOptions> _activeUnitOfWork;

        #region Constructor
        public UnitOfWork(
            ILoggerFactory loggerFactory, 
            ApplicationDbContext dbContext,
            IDbService dbService)
        {
            _loggerFactory = loggerFactory;
            _dbContext = dbContext;
            _dbService = dbService;
            
            UseProvider((int) DataAccessProvider.Dapper );
        }
        #endregion

        #region Repositories
        public IAccountRepository<IRepositoryOptions> Accounts 
            => _activeUnitOfWork.Accounts;
        public IAllocationRepository<IRepositoryOptions> Allocations 
            => _activeUnitOfWork.Allocations;
        public IBudgetRepository<IRepositoryOptions> Budgets 
            => _activeUnitOfWork.Budgets;
        public IBudgetTargetRepository<IRepositoryOptions> BudgetTargets 
            => _activeUnitOfWork.BudgetTargets;
        public IEconomicMacroRepository<IRepositoryOptions> EconomicMacros
            => _activeUnitOfWork.EconomicMacros;
        public IMediaFileRepository<IRepositoryOptions> MediaFiles 
            => _activeUnitOfWork.MediaFiles;
        public ISignInAttemptRepository<IRepositoryOptions> SignInAttempts 
            => _activeUnitOfWork.SignInAttempts;
        public ISignInSessionRepository<IRepositoryOptions> SignInSessions
            => _activeUnitOfWork.SignInSessions;
        public ISimulationConfigRepository<IRepositoryOptions> SimulationConfigs 
            => _activeUnitOfWork.SimulationConfigs;
        public ISimulationSessionRepository<IRepositoryOptions> SimulationSessions 
            => _activeUnitOfWork.SimulationSessions;
        public ISimulationShareRepository<IRepositoryOptions> SimulationShares
            => _activeUnitOfWork.SimulationShares;
        public ISimulationSpecialPolicyAllocationRepository<IRepositoryOptions> SimulationSpecialPolicyAllocations 
            => _activeUnitOfWork.SimulationSpecialPolicyAllocations;
        public ISimulationStateExpenditureRepository<IRepositoryOptions> SimulationStateExpenditures 
            => _activeUnitOfWork.SimulationStateExpenditures;
        public ISimulationEconomicMacroRepository<IRepositoryOptions> SimulationEconomicMacros
            => _activeUnitOfWork.SimulationEconomicMacros;
        public ISpecialPolicyRepository<IRepositoryOptions> SpecialPolicies 
            => _activeUnitOfWork.SpecialPolicies;
        public ISpecialPolicyAllocationRepository<IRepositoryOptions> SpecialPolicyAllocations 
            => _activeUnitOfWork.SpecialPolicyAllocations;
        public IStateBudgetRepository<IRepositoryOptions> StateBudgets 
            => _activeUnitOfWork.StateBudgets;
        public IStateExpenditureRepository<IRepositoryOptions> StateExpenditures 
            => _activeUnitOfWork.StateExpenditures;
        public IStateExpenditureAllocationRepository<IRepositoryOptions> StateExpenditureAllocations 
            => _activeUnitOfWork.StateExpenditureAllocations;
        public ITokenRepository<IRepositoryOptions> Tokens 
            => _activeUnitOfWork.Tokens;
        public IWebContentRepository<IRepositoryOptions> WebContents
            => _activeUnitOfWork.WebContents;
        #endregion

        #region IUnitOfWork Implementation
        public IDbConnection DbConnection => _activeUnitOfWork.DbConnection;
        public IDbTransaction DbTransaction => _activeUnitOfWork.DbTransaction;
        public bool IsUsingTransaction => _activeUnitOfWork.IsUsingTransaction;

        public void BeginTransaction()
        {
            _activeUnitOfWork.BeginTransaction();
        }

        public void Commit()
        {
            _activeUnitOfWork.Commit();
        }

        public Task CommitAsync()
        {
            return _activeUnitOfWork.CommitAsync();
        }

        public void Rollback()
        {
            _activeUnitOfWork.Rollback();
        }

        public Task RollbackAsync()
        {
            return _activeUnitOfWork.RollbackAsync();
        }

        public void UseProvider(int provider)
        {
            _activeUnitOfWork = (DataAccessProvider) provider switch
            {
                DataAccessProvider.EntityFrameworkCore 
                    => new EntityFrameworkCore.UnitOfWork(this, _loggerFactory, _dbContext),
                DataAccessProvider.Dapper 
                    => new Dapper.UnitOfWork( this, _loggerFactory, _dbService),
                _ => _activeUnitOfWork
            };
        }

        public void Dispose()
        {
            _activeUnitOfWork.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _activeUnitOfWork.DisposeAsync();
        }
        #endregion
    }
}