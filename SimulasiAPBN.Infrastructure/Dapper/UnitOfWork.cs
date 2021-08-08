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
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;
using SimulasiAPBN.Application.Repositories;
using SimulasiAPBN.Infrastructure.Dapper.Repositories;

namespace SimulasiAPBN.Infrastructure.Dapper
{
    public class UnitOfWork : IUnitOfWork<RepositoryOptions>
    {
        private readonly IUnitOfWork<IRepositoryOptions> _parent;
        private readonly ILoggerFactory _loggerFactory;
        private bool _disposed;

        #region Constructor
        public UnitOfWork(IUnitOfWork<IRepositoryOptions> parent, ILoggerFactory loggerFactory, IDbService dbService)
        {
            _parent = parent;
            _loggerFactory = loggerFactory;
            _disposed = false;

            DbConnection = dbService.SqlConnection;
            ResetRepositories();
        }
        #endregion

        #region Helper Methods
        private void ResetRepositories()
        {
            _accounts = null;
            _allocations = null;
            _budgets = null;
            _budgetTargets = null;
            _economicMacros = null;
            _mediaFiles = null;
            _signInAttempts = null;
            _simulationConfigs = null;
            _simulationSessions = null;
            _simulationSpecialPolicyAllocations = null;
            _simulationStateExpenditures = null;
            _simulationEconomicMacros = null;
            _specialPolicies = null;
            _specialPolicyAllocations = null;
            _stateBudgets = null;
            _stateExpenditures = null;
            _stateExpenditureAllocations = null;
            _tokens = null;
            _webContents = null;
        }
        #endregion
        
        #region Helper Properties
        private RepositoryOptions RepositoryOptions 
            => new RepositoryOptions(_loggerFactory, DbConnection, DbTransaction);
        #endregion
        
        #region Repositories
        private IAccountRepository<RepositoryOptions>? _accounts;
        public IAccountRepository<RepositoryOptions> Accounts 
            => _accounts ??= new AccountRepository(RepositoryOptions); 

        private IAllocationRepository<RepositoryOptions>? _allocations;
        public IAllocationRepository<RepositoryOptions> Allocations 
            => _allocations ??= new AllocationRepository(RepositoryOptions);


        private IEconomicMacroRepository<RepositoryOptions>? _economicMacros;
        public IEconomicMacroRepository<RepositoryOptions> EconomicMacros
            => _economicMacros ??= new EconomicMacroRepository(RepositoryOptions);


        private IBudgetRepository<RepositoryOptions>? _budgets;
        public IBudgetRepository<RepositoryOptions> Budgets 
        => _budgets ??= new BudgetRepository(RepositoryOptions);
        
        
        private IBudgetTargetRepository<RepositoryOptions>? _budgetTargets;
        public IBudgetTargetRepository<RepositoryOptions> BudgetTargets 
            => _budgetTargets ??= new BudgetTargetRepository(RepositoryOptions);
        
        
        private IMediaFileRepository<RepositoryOptions>? _mediaFiles;
        public IMediaFileRepository<RepositoryOptions> MediaFiles
            => _mediaFiles ??= new MediaFileRepository(RepositoryOptions);
        
        
        private ISignInAttemptRepository<RepositoryOptions>? _signInAttempts;
        public ISignInAttemptRepository<RepositoryOptions> SignInAttempts 
            => _signInAttempts ??= new SignInAttemptRepository(RepositoryOptions);
        
        private ISignInSessionRepository<RepositoryOptions>? _signInSessions;
        public ISignInSessionRepository<RepositoryOptions> SignInSessions
            => _signInSessions ??= new SignInSessionRepository(RepositoryOptions);
        
        private ISimulationConfigRepository<RepositoryOptions>? _simulationConfigs;
        public ISimulationConfigRepository<RepositoryOptions> SimulationConfigs 
            => _simulationConfigs ??= new SimulationConfigRepository(RepositoryOptions);
        
        private ISimulationSessionRepository<RepositoryOptions>? _simulationSessions;
        public ISimulationSessionRepository<RepositoryOptions> SimulationSessions 
            => _simulationSessions ??= new SimulationSessionRepository(RepositoryOptions);

        private ISimulationShareRepository<RepositoryOptions>? _simulationShares;
        public ISimulationShareRepository<RepositoryOptions> SimulationShares
            => _simulationShares ??= new SimulationShareRepository(RepositoryOptions);

        private ISimulationSpecialPolicyAllocationRepository<RepositoryOptions>? _simulationSpecialPolicyAllocations;
        public ISimulationSpecialPolicyAllocationRepository<RepositoryOptions> SimulationSpecialPolicyAllocations 
            => _simulationSpecialPolicyAllocations ??= new SimulationSpecialPolicyAllocationRepository(RepositoryOptions);

        private ISimulationStateExpenditureRepository<RepositoryOptions>? _simulationStateExpenditures;
        public ISimulationStateExpenditureRepository<RepositoryOptions> SimulationStateExpenditures 
            => _simulationStateExpenditures ??= new SimulationStateExpenditureRepository(RepositoryOptions);

        private ISimulationEconomicMacroRepository<RepositoryOptions>? _simulationEconomicMacros;
        public ISimulationEconomicMacroRepository<RepositoryOptions> SimulationEconomicMacros
            => _simulationEconomicMacros ??= new SimulationEconomicMacroRepository(RepositoryOptions);

        private ISpecialPolicyRepository<RepositoryOptions>? _specialPolicies;
        public ISpecialPolicyRepository<RepositoryOptions> SpecialPolicies
            => _specialPolicies ??= new SpecialPolicyRepository(RepositoryOptions);

        private ISpecialPolicyAllocationRepository<RepositoryOptions>? _specialPolicyAllocations;
        public ISpecialPolicyAllocationRepository<RepositoryOptions> SpecialPolicyAllocations 
            => _specialPolicyAllocations ??= new SpecialPolicyAllocationRepository(RepositoryOptions);

        private IStateBudgetRepository<RepositoryOptions>? _stateBudgets;
        public IStateBudgetRepository<RepositoryOptions> StateBudgets 
            => _stateBudgets ??= new StateBudgetRepository(RepositoryOptions);

        private IStateExpenditureRepository<RepositoryOptions>? _stateExpenditures;
        public IStateExpenditureRepository<RepositoryOptions> StateExpenditures 
            => _stateExpenditures ??= new StateExpenditureRepository(RepositoryOptions);

        private IStateExpenditureAllocationRepository<RepositoryOptions>? _stateExpenditureAllocations;
        public IStateExpenditureAllocationRepository<RepositoryOptions> StateExpenditureAllocations 
            => _stateExpenditureAllocations ??= new StateExpenditureAllocationRepository(RepositoryOptions);

        private ITokenRepository<RepositoryOptions>? _tokens;
        public ITokenRepository<RepositoryOptions> Tokens 
            => _tokens ??= new TokenRepository(RepositoryOptions);
        
        private IWebContentRepository<RepositoryOptions>? _webContents;
        public IWebContentRepository<RepositoryOptions> WebContents 
            => _webContents ??= new WebContentRepository(RepositoryOptions);
        #endregion

        #region IUnitOfWork Implementation
        public IDbConnection DbConnection { get; }
        public IDbTransaction? DbTransaction { get; private set; }
        public bool IsUsingTransaction => DbTransaction is not null;

        public void BeginTransaction()
        {
            DbTransaction = DbConnection.BeginTransaction();
            ResetRepositories();
        }

        public void Commit()
        {
            if (DbTransaction is null) return;
            
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        public Task CommitAsync()
        {
            return Task.Run(Commit);
        }

        public void Rollback()
        {
            if (DbTransaction is null) return;
            
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        public Task RollbackAsync()
        {
            return Task.Run(Rollback);
        }

        public void UseProvider(int provider)
        {
            _parent.UseProvider(provider);
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            if (DbTransaction is not null)
            {
                DbTransaction.Dispose();
                DbTransaction = null;
            }
            DbConnection.Dispose();
            
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
        #endregion
    }
}