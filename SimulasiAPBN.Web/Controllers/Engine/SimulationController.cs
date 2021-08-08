/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Configuration;
using SimulasiAPBN.Core.Common;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Controllers.Engine.Common;
using SimulasiAPBN.Web.Models;
using SimulasiAPBN.Web.Models.Engine;
using SimulasiAPBN.Web.Validation;

// ReSharper disable PossibleMultipleEnumeration
namespace SimulasiAPBN.Web.Controllers.Engine
{
    public class SimulationController : EngineController
    {
        public SimulationController( IUnitOfWork unitOfWork, IValidatorFactory validatorFactory) 
            : base(unitOfWork, validatorFactory) { }

        private async Task<ClientSimulation> GetClientSimulation(Guid sessionGuid)
        {
            // SimulationSession, linked: StateBudget
            var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
            if (session is null)
            {
                return null;
            }

            var allocations = await UnitOfWork.Allocations.GetAllAsync();
            // Budget, linked: BudgetTarget
            var budgets = await UnitOfWork.Budgets.GetAllAsync();

            // StateBudget, linked: SpecialPolicy, StateExpenditure, Budget
            var stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(session.StateBudgetId);
            if (stateBudget is null)
            {
                return null;
            }
            stateBudget.SpecialPolicies = stateBudget.SpecialPolicies
                .Where(policy => policy.IsActive)
                .ToList();
            
            // SimulationStateExpenditure, linked: StateExpenditure
            var simulationStateExpenditures = new Collection<SimulationStateExpenditure>();
            foreach (var simulationStateExpenditure in await UnitOfWork.SimulationStateExpenditures
                .GetBySimulationSessionAsync(session))
            {
                var expenditure = simulationStateExpenditure.StateExpenditure;
                
                // StateExpenditureAllocation, linked: Allocation
                var expenditureAllocations = await UnitOfWork
                    .StateExpenditureAllocations
                    .GetByStateExpenditureAsync(expenditure);
                foreach (var expenditureAllocation in expenditureAllocations)
                {
                    var allocationId = expenditureAllocation.AllocationId;
                    expenditureAllocation.Allocation = null;
                    expenditureAllocation.AllocationId = allocationId;
                }

                expenditure.Budget = budgets.FirstOrDefault(budget => budget.Id == expenditure.BudgetId);
                expenditure.StateExpenditureAllocations = expenditureAllocations.ToList();

                simulationStateExpenditure.StateExpenditure = expenditure;

                simulationStateExpenditures.Add(simulationStateExpenditure);
            }
            
            // SimulationSpecialPolicyAllocations, linked: SpecialPolicyAllocation
            var simulationSpecialPolicyAllocations = new Collection<SimulationSpecialPolicyAllocation>();
            foreach (var policyAllocation in await UnitOfWork.SimulationSpecialPolicyAllocations
                .GetBySimulationSessionAsync(session))
            {
                var specialPolicy = stateBudget.SpecialPolicies
                    .FirstOrDefault(policy => policy.Id == policyAllocation.SpecialPolicyAllocation.SpecialPolicyId);
                if (specialPolicy is null)
                {
                    continue;
                }
                
                var allocationId = policyAllocation.SpecialPolicyAllocation.AllocationId;
                policyAllocation.SpecialPolicyAllocation.Allocation = null;
                policyAllocation.SpecialPolicyAllocation.AllocationId = allocationId;
                
                simulationSpecialPolicyAllocations.Add(policyAllocation);
            }

            // SimulationEconomicMacros, linked: EconomicMacro
            var economicMacros = UnitOfWork.EconomicMacros.GetByStateBudget(stateBudget);
            var simulationEconomicMacros = new Collection<SimulationEconomicMacro>();
            foreach (var simulationEconomic in await UnitOfWork.SimulationEconomicMacros
                .GetBySimulationSessionAsync(session))
            {
                simulationEconomicMacros.Add(simulationEconomic);
            }

            stateBudget.EconomicMacros = new Collection<EconomicMacro>();
            stateBudget.StateExpenditures = new Collection<StateExpenditure>();
            session.StateBudget = stateBudget;
            session.SimulationStateExpenditures = simulationStateExpenditures;
            session.SimulationSpecialPolicyAllocations = simulationSpecialPolicyAllocations;
            session.SimulationEconomicMacros = simulationEconomicMacros;
            

            return new ClientSimulation
            {
                Allocations = allocations,
                Economics = economicMacros,
                Session = session
            };
        }
        
        [HttpGet]
        [Route("WebContent")]
        public async Task<EngineResponse> GetWebContents()
        {
            var webContents = await UnitOfWork.WebContents.GetAllAsync();
            return new EngineResponse(webContents);
        }
        
        
        [HttpGet]
        [Route("Config/DeficitLaw")]
        public async Task<EngineResponse> GetDeficitLaw()
        {
            var deficitLaw = await UnitOfWork.SimulationConfigs.GetDeficitLawAsync();
            return new EngineResponse(deficitLaw);
        }
        
        [HttpGet]
        [Route("Config/DeficitThreshold")]
        public async Task<EngineResponse> GetDeficitThreshold()
        {
            var deficitThreshold = await UnitOfWork.SimulationConfigs.GetDeficitThresholdAsync();
            return new EngineResponse(deficitThreshold);
        }
        
        [HttpGet]
        [Route("Config/GrossDomesticProduct")]
        public async Task<EngineResponse> GetGrossDomesticProductAsync()
        {
            var grossDomesticProduct = await UnitOfWork.SimulationConfigs.GetGrossDomesticProductAsync();
            return new EngineResponse(grossDomesticProduct);
        }
        
        [HttpGet]
        [Route("Rules")]
        public async Task<EngineResponse> GetRules()
        {
            var rules = new Collection<string>();
            
            var allocations = await UnitOfWork.Allocations
                .FindAsync(allocation => allocation.IsMandatory);
            foreach (var allocation in allocations)
            {
                rules.Add($"Alokasi Anggaran {allocation.Name} minimum {allocation.MandatoryThreshold}% " +
                          $"dari Total Belanja Negara");
            }

            var deficitThreshold = await UnitOfWork.SimulationConfigs.GetDeficitThresholdAsync();
            rules.Add($"Defisit APBN yang jumlahnya kurang dari {deficitThreshold}% terhadap Produk Domestik Bruto");
            
            var debtRatio = await UnitOfWork.SimulationConfigs.GetDebtRatioAsync();
            rules.Add($"Rasio utang yang jumlahnya kurang dari {debtRatio}% terhadap Produk Domestik Bruto");
            
            return new EngineResponse(rules);
        }

        [HttpPost]
        [Route("Session")]
        public async Task<EngineResponse> CreateSession(SimulationSession model)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                (await Validator.Session.GetValidationAsync(model))
                    .ThrowIfInvalid();

                var stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
                if (stateBudget is null)
                {
                    // If there is no StateBudget for current year, get from previous year
                    var previousYear = DateTime.Now.Year - 1;
                    stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
                    if (stateBudget is null)
                    {
                        // No StateBudget available
                        throw new ServiceUnavailableException(
                            $"Belum ada data Kebijakan APBN yang berlaku untuk Tahun Anggaran {DateTimeOffset.Now.Year}.",
                            EngineErrorCode.NoStateBudgetData);
                    }
                }
                stateBudget.SpecialPolicies = stateBudget.SpecialPolicies
                    .Where(policy => policy.IsActive)
                    .ToList();

                var session = new SimulationSession()
                {
                    Name = model.Name,
                    EngineKey = EasyRandom.GeneratePassword(32),
                    StateBudgetId = stateBudget.Id,
                    UsedIncome = stateBudget.CountryIncome,
                    SimulationState = SimulationState.Started
                };
                await UnitOfWork.SimulationSessions.AddAsync(session);

                var allocations = await UnitOfWork.Allocations.GetAllAsync();
                // Budget, linked: BudgetTarget
                var budgets = await UnitOfWork.Budgets.GetAllAsync();
                var economicMacros = await UnitOfWork.EconomicMacros.GetAllAsync();
               
                var simulationStateExpenditures = new Collection<SimulationStateExpenditure>();
                foreach (var expenditure in stateBudget.StateExpenditures)
                {
                    // StateExpenditureAllocation, linked: Allocation
                    var expenditureAllocations = await UnitOfWork
                        .StateExpenditureAllocations
                        .GetByStateExpenditureAsync(expenditure);
                    foreach (var expenditureAllocation in expenditureAllocations)
                    {
                        var allocationId = expenditureAllocation.AllocationId;
                        expenditureAllocation.Allocation = null;
                        expenditureAllocation.AllocationId = allocationId;
                    }

                    expenditure.Budget = budgets.FirstOrDefault(budget => budget.Id == expenditure.BudgetId);
                    expenditure.StateExpenditureAllocations = expenditureAllocations.ToList();

                    simulationStateExpenditures.Add(new SimulationStateExpenditure
                    {
                        SimulationSessionId = session.Id,
                        StateExpenditure = expenditure,
                        TotalAllocation = expenditure.TotalAllocation,
                        IsPriority = false,
                    });
                }

                await UnitOfWork.SimulationStateExpenditures.AddRangeAsync(simulationStateExpenditures);

                var simulationEconomicMacros = new Collection<SimulationEconomicMacro>();
                foreach (var economicMacro in economicMacros)
                {
                    simulationEconomicMacros.Add(new SimulationEconomicMacro
                    {
                        SimulationSessionId = session.Id,
                        EconomicMacro = economicMacro,
                        UsedValue = economicMacro.DefaultValue,
                    });
                }

                await UnitOfWork.SimulationEconomicMacros.AddRangeAsync(simulationEconomicMacros);


                var simulationSpecialPolicyAllocations = new List<SimulationSpecialPolicyAllocation>();
                foreach (var policy in stateBudget.SpecialPolicies)
                {
                    // SpecialPolicy, linked: SpecialPolicyAllocation, Allocation
                    var completePolicy = await UnitOfWork.SpecialPolicies.GetByIdAsync(policy.Id);
                    if (completePolicy is null || !completePolicy.IsActive)
                    {
                        continue;
                    }

                    var policyAllocations = completePolicy
                        .SpecialPolicyAllocations
                        .Select(policyAllocation =>
                        {
                            var allocationId = policyAllocation.AllocationId;
                            policyAllocation.Allocation = null;
                            policyAllocation.AllocationId = allocationId;
                            
                            return new SimulationSpecialPolicyAllocation
                            {
                                SimulationSessionId = session.Id,
                                SpecialPolicyAllocation = policyAllocation,
                                TotalAllocation = policyAllocation.TotalAllocation
                            };
                        });

                    simulationSpecialPolicyAllocations.AddRange(policyAllocations);
                }

                await UnitOfWork.SimulationSpecialPolicyAllocations.AddRangeAsync(simulationSpecialPolicyAllocations);

                stateBudget.StateExpenditures = new Collection<StateExpenditure>();
                stateBudget.EconomicMacros = new Collection<EconomicMacro>();
                session.StateBudget = stateBudget;
                session.SimulationStateExpenditures = simulationStateExpenditures;
                session.SimulationSpecialPolicyAllocations = simulationSpecialPolicyAllocations;
                session.SimulationEconomicMacros = simulationEconomicMacros;

                await UnitOfWork.CommitAsync();

                var cookieOptions = new CookieOptions
                {
                    Expires = SimulationSessionKey.CookieExpires,
                    HttpOnly = SimulationSessionKey.CookieHttpOnly,
                    MaxAge = SimulationSessionKey.MaxAge,
                    SameSite = SameSiteMode.Strict,
                    Secure = SimulationSessionKey.CookieSecure
                };
                Response.Cookies.Append(SimulationSessionKey.CookieName, session.EngineKey, cookieOptions);

                return new EngineResponse(new ClientSimulation
                {
                    Allocations = allocations,
                    Economics = economicMacros,
                    Session = session
                });
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpGet]
        [Route("Session")]
        public async Task<EngineResponse> GetSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new BadRequestException(
                    "Identitas sesi diperlukan, namun tidak ada dalam parameter.",
                    EngineErrorCode.RequiredDataNotProvided);
            }

            if (!Guid.TryParse(sessionId, out var sessionGuid))
            {
                throw new BadRequestException(
                     "Format identitas sesi tidak valid.",
                    EngineErrorCode.InvalidDataFormat);
            }

            var clientSimulation = await GetClientSimulation(sessionGuid);
            if (clientSimulation is null)
            {
                throw new NotFoundException(
                    "Data sesi tersebut tidak ditemukan.",
                    EngineErrorCode.DataNotFound);
            }

            if (!Request.Cookies.TryGetValue(SimulationSessionKey.CookieName, out var sessionKey))
            {
                throw new BadRequestException(
                    "Kunci sesi simulasi diperlukan, namun tidak ada dalam parameter.",
                    EngineErrorCode.SessionKeyNotProvided);
            }

            if (sessionKey != clientSimulation.Session.EngineKey)
            {
                throw new BadRequestException(
                    "Kunci sesi simulasi tidak valid.",
                    EngineErrorCode.InvalidSessionKey);
            }

            return new EngineResponse(clientSimulation);
        }

        [HttpPatch]
        [Route("StateExpenditure")]
        public async Task<EngineResponse> UpdateStateExpenditure(string sessionId, 
            DataModel<SimulationStateExpenditure> dataModel)
        {
            // Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                // Validate the model
                foreach (var model in dataModel.Data)
                {
                    (await Validator.SimulationStateExpenditure.GetValidationAsync(model))
                        .ThrowIfInvalid();
                }
                
                // Verify and parse Session ID
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new BadRequestException(
                        "Identitas sesi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.RequiredDataNotProvided);
                }

                if (!Guid.TryParse(sessionId, out var sessionGuid))
                {
                    throw new BadRequestException(
                        "Format identitas sesi tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }

                // Get the Session
                var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
                if (session is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
                
                // Check if the Session has completed
                if (session.SimulationState == SimulationState.Completed)
                {
                    throw new BadRequestException(
                        "Sesi simulasi telah berakhir.",
                        EngineErrorCode.SessionHasBeenCompleted);
                }
                
                // Validate the Session engine key
                if (!Request.Cookies.TryGetValue(SimulationSessionKey.CookieName, out var sessionKey))
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.SessionKeyNotProvided);
                }

                if (sessionKey != session.EngineKey)
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi tidak valid.",
                        EngineErrorCode.InvalidSessionKey);
                }
                
                foreach (var model in dataModel.Data)
                {
                    // Get the Expenditure
                    var expenditure = await UnitOfWork.SimulationStateExpenditures
                        .FindOneAsync(entity => entity.Id == model.Id &&
                                                entity.SimulationSessionId == session.Id);
                    if (expenditure is null)
                    {
                        throw new NotFoundException(
                            "Data belanja negara tersebut tidak ditemukan.",
                            EngineErrorCode.DataNotFound);
                    }

                    // Check if the total allocation exceeds maximum allowed by the multiplier
                    var maximumTotalAllocation = expenditure.StateExpenditure.TotalAllocation *
                                                 expenditure.StateExpenditure.SimulationMaximumMultiplier;
                    if (model.TotalAllocation > maximumTotalAllocation)
                    {
                        var budget = await UnitOfWork.Budgets.GetByIdAsync(expenditure.StateExpenditure.BudgetId);
                        if (budget is null)
                        {
                            throw new NotFoundException(
                                "Data anggaran tersebut tidak ditemukan.",
                                EngineErrorCode.DataNotFound);
                        }

                        throw new BadRequestException(
                            $"Total alokasi {budget.Function} maksimum Rp {maximumTotalAllocation} T.",
                            EngineErrorCode.DataValidationFailed);
                    }
                
                    // Make the changes
                    expenditure.TotalAllocation = model.TotalAllocation;

                    await UnitOfWork.SimulationStateExpenditures.ModifyAsync(expenditure);
                }
                
                // Calculating priorities of all Simulation State Expenditures
                var priorityExpenditures = new Dictionary<SimulationStateExpenditure, decimal[]>();

                var simulationStateExpenditures = await UnitOfWork
                    .SimulationStateExpenditures
                    .GetBySimulationSessionAsync(session);
                foreach (var simulationStateExpenditure in simulationStateExpenditures)
                {
                    simulationStateExpenditure.IsPriority = false;
                    await UnitOfWork.SimulationStateExpenditures.ModifyAsync(simulationStateExpenditure);
                    
                    var initialTotalAllocation = simulationStateExpenditure.StateExpenditure.TotalAllocation;
                    var totalAllocation = simulationStateExpenditure.TotalAllocation;
                    
                    if (totalAllocation <= initialTotalAllocation)
                    {
                        continue;
                    }

                    var ratio = totalAllocation / initialTotalAllocation;
                    priorityExpenditures.Add(simulationStateExpenditure, new []{ ratio, totalAllocation });
                }

                var topPriorityExpenditures = priorityExpenditures
                    .OrderByDescending(pair => pair.Value[0])
                    .ThenByDescending(pair => pair.Value[1])
                    .Take(3);
                foreach (var priorityExpenditure in topPriorityExpenditures)
                {
                    var simulationStateExpenditure = priorityExpenditure.Key;
                    simulationStateExpenditure.IsPriority = true;
                    await UnitOfWork.SimulationStateExpenditures.ModifyAsync(simulationStateExpenditure);
                }
                
                session.SimulationState = SimulationState.OnProgress;
                await UnitOfWork.SimulationSessions.ModifyAsync(session);

                // Get the client simulation session
                var clientSimulation = await GetClientSimulation(session.Id);
                if (clientSimulation is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
                
                // Commit and return response
                await UnitOfWork.CommitAsync();

                return new EngineResponse(clientSimulation);
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();
                throw;
            }
        }
        
        [HttpPatch]
        [Route("SpecialPolicyAllocation")]
        public async Task<EngineResponse> UpdateSpecialPolicyAllocation(string sessionId, 
            DataModel<SimulationSpecialPolicyAllocation> dataModel) 
        {
            // Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                // Validate the model
                foreach (var model in dataModel.Data)
                {
                    (await Validator.SimulationSpecialPolicyAllocation.GetValidationAsync(model))
                        .ThrowIfInvalid();
                }
                
                // Verify and parse Session ID
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new BadRequestException(
                        "Identitas sesi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.RequiredDataNotProvided);
                }

                if (!Guid.TryParse(sessionId, out var sessionGuid))
                {
                    throw new BadRequestException(
                        "Format identitas sesi tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }
                
                // Get the Session
                var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
                if (session is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
                
                // Check if the Session has completed
                if (session.SimulationState == SimulationState.Completed)
                {
                    throw new BadRequestException(
                        "Sesi simulasi telah berakhir.",
                        EngineErrorCode.SessionHasBeenCompleted);
                }
                
                // Validate the Session engine key
                if (!Request.Cookies.TryGetValue(SimulationSessionKey.CookieName, out var sessionKey))
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.SessionKeyNotProvided);
                }

                if (sessionKey != session.EngineKey)
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi tidak valid.",
                        EngineErrorCode.InvalidSessionKey);
                }

                foreach (var model in dataModel.Data)
                {
                    // Get the Policy Allocation
                    var policyAllocation = await UnitOfWork.SimulationSpecialPolicyAllocations
                        .FindOneAsync(entity => entity.Id == model.Id &&
                                                entity.SimulationSessionId == session.Id);
                    if (policyAllocation is null)
                    {
                        throw new NotFoundException(
                            "Data alokasi kebijakan khusus tersebut tidak ditemukan.",
                            EngineErrorCode.DataNotFound);
                    }
                
                    // Check if the total allocation exceeds maximum allowed by the multiplier
                    var maximumTotalAllocation = policyAllocation.SpecialPolicyAllocation.TotalAllocation *
                                                 policyAllocation.SpecialPolicyAllocation.SimulationMaximumMultiplier;
                    if (model.TotalAllocation > maximumTotalAllocation)
                    {
                        var allocation = await UnitOfWork.Allocations
                            .GetByIdAsync(policyAllocation.SpecialPolicyAllocation.AllocationId);
                        if (allocation is null)
                        {
                            throw new NotFoundException(
                                "Data alokasi anggaran tersebut tidak ditemukan.",
                                EngineErrorCode.DataNotFound);
                        }

                        var specialPolicy = await UnitOfWork.SpecialPolicies
                            .GetByIdAsync(policyAllocation.SpecialPolicyAllocation.SpecialPolicyId);
                        if (specialPolicy is null)
                        {
                            throw new NotFoundException(
                                "Data kebijakan khusus tersebut tidak ditemukan.",
                                EngineErrorCode.DataNotFound);
                        }


                        throw new BadRequestException(
                            $"Total alokasi {allocation.Name} pada {specialPolicy.Name} " +
                            $"maksimum Rp {maximumTotalAllocation} T.",
                            EngineErrorCode.DataValidationFailed);
                    }
                    
                    policyAllocation.TotalAllocation = model.TotalAllocation;
                    await UnitOfWork.SimulationSpecialPolicyAllocations.ModifyAsync(policyAllocation);
                }
                
                // Make the changes
                session.SimulationState = SimulationState.OnProgress;
                await UnitOfWork.SimulationSessions.ModifyAsync(session);
                
                // Get the client simulation session
                var clientSimulation = await GetClientSimulation(session.Id);
                if (clientSimulation is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
                
                // Commit and return response
                await UnitOfWork.CommitAsync();

                return new EngineResponse(clientSimulation);
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpPatch]
        [Route("EconomicMacro")]
        public async Task<EngineResponse> UpdateEconomicMacro(string sessionId, decimal newTotalEconomicMacro,
            DataModel<SimulationEconomicMacro> dataModel)
        {
            // Begin transaction
            UnitOfWork.BeginTransaction();
            try
            {
                // Validate the model
                foreach (var model in dataModel.Data)
                {
                    (await Validator.SimulationEconomicMacro.GetValidationAsync(model))
                        .ThrowIfInvalid();
                }

                // Verify and parse Session ID
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new BadRequestException(
                        "Identitas sesi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.RequiredDataNotProvided);
                }

                if (!Guid.TryParse(sessionId, out var sessionGuid))
                {
                    throw new BadRequestException(
                        "Format identitas sesi tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }

                // Get the Session
                var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
                if (session is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }

                // Check if the Session has completed
                if (session.SimulationState == SimulationState.Completed)
                {
                    throw new BadRequestException(
                        "Sesi simulasi telah berakhir.",
                        EngineErrorCode.SessionHasBeenCompleted);
                }

                // Validate the Session engine key
                if (!Request.Cookies.TryGetValue(SimulationSessionKey.CookieName, out var sessionKey))
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.SessionKeyNotProvided);
                }

                if (sessionKey != session.EngineKey)
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi tidak valid.",
                        EngineErrorCode.InvalidSessionKey);
                }

                foreach (var model in dataModel.Data)
                {
                    // Get the Expenditure
                    var economicMacro = await UnitOfWork.SimulationEconomicMacros
                        .FindOneAsync(entity => entity.Id == model.Id &&
                                                entity.SimulationSessionId == session.Id);
                    if (economicMacro is null)
                    {
                        throw new NotFoundException(
                            "Data ekonomi makro tersebut tidak ditemukan.",
                            EngineErrorCode.DataNotFound);
                    }


                    // Make the changes
                    economicMacro.UsedValue = model.UsedValue;
                    await UnitOfWork.SimulationEconomicMacros.ModifyAsync(economicMacro);
                }

                // Calculating priorities of all Simulation Economic Macro
                var priorityEconomicMacros = new Dictionary<SimulationEconomicMacro, decimal[]>();

                var simulationEconomicMacros = await UnitOfWork
                    .SimulationEconomicMacros
                    .GetBySimulationSessionAsync(session);
                var totalBudgetEconomic = session.StateBudget.CountryIncome;
                foreach (var simulationEconomicMacro in simulationEconomicMacros)
                {
                    var usedValue = simulationEconomicMacro.UsedValue;
                    var defaultValue = simulationEconomicMacro.EconomicMacro.DefaultValue;
                    var threshold = simulationEconomicMacro.EconomicMacro.Threshold;
                    var thresholdValue = simulationEconomicMacro.EconomicMacro.ThresholdValue;

                    var diffValue = (usedValue - defaultValue) * thresholdValue;
                    
                    if (usedValue == defaultValue)
                    {
                        continue;
                    }

                    totalBudgetEconomic += diffValue / threshold / 1000 ;
                    
                    //await UnitOfWork.SimulationEconomicMacros.ModifyAsync(simulationEconomicMacro);
                }
                session.UsedIncome = newTotalEconomicMacro;
                session.SimulationState = SimulationState.OnProgress;
                await UnitOfWork.SimulationSessions.ModifyAsync(session);

                // Get the client simulation session
                var clientSimulation = await GetClientSimulation(session.Id);
                if (clientSimulation is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
                
                // Commit and return response
                await UnitOfWork.CommitAsync();

                return new EngineResponse(clientSimulation);
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();
                throw;
            }
        }


        [HttpDelete]
        [Route("Session")]
        public async Task<EngineResponse> FinishSession(string sessionId)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                // Verify and parse Session ID
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new BadRequestException(
                        "Identitas sesi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.RequiredDataNotProvided);
                }

                if (!Guid.TryParse(sessionId, out var sessionGuid))
                {
                    throw new BadRequestException(
                        "Format identitas sesi tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }

                // Get the Session
                var session = await UnitOfWork.SimulationSessions.GetByIdAsync(sessionGuid);
                if (session is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }

                // Check if the Session has completed
                if (session.SimulationState == SimulationState.Completed)
                {
                    throw new BadRequestException(
                        "Sesi simulasi telah berakhir.",
                        EngineErrorCode.SessionHasBeenCompleted);
                }

                // Validate the Session engine key
                if (!Request.Cookies.TryGetValue(SimulationSessionKey.CookieName, out var sessionKey))
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi diperlukan, namun tidak ada dalam parameter.",
                        EngineErrorCode.SessionKeyNotProvided);
                }

                if (sessionKey != session.EngineKey)
                {
                    throw new BadRequestException(
                        "Kunci sesi simulasi tidak valid.",
                        EngineErrorCode.InvalidSessionKey);
                }

                // Marks as Completed
                session.SimulationState = SimulationState.Completed;
                await UnitOfWork.SimulationSessions.ModifyAsync(session);

                var clientSimulation = await GetClientSimulation(sessionGuid);
                if (clientSimulation is null)
                {
                    throw new NotFoundException(
                        "Data sesi tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }

                await UnitOfWork.CommitAsync();
                
                Response.Cookies.Delete(SimulationSessionKey.CookieName);

                return new EngineResponse(clientSimulation);
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();
                throw;
            }
        }

    }
}
