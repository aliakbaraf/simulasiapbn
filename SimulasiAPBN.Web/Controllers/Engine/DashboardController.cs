/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Extensions;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Controllers.Engine.Common;
using SimulasiAPBN.Web.Models.Engine;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Controllers.Engine
{
    [Authorize]
    public class DashboardController : EngineController
    {
        public DashboardController(
            IUnitOfWork unitOfWork, 
            IValidatorFactory validatorFactory) 
            : base(unitOfWork, validatorFactory)
        {
        }
        
        [HttpGet]
        [Route("Allocations")]
        public async Task<EngineResponse> GetAllocations()
        {
            var allocations = await UnitOfWork.Allocations.GetAllAsync();
            return new EngineResponse(allocations);
        }
        
        [HttpGet]
        [Route("Budgets")]
        public async Task<EngineResponse> GetBudgets()
        {
            var budgets = await UnitOfWork.Budgets.GetAllAsync();
            return new EngineResponse(budgets);
        }

        [HttpGet]
        [Route("EconomicMacros")]
        public Task<EngineResponse> GetEconomicMacros()
        {
            return GetEconomicMacros(string.Empty);
        }

        [HttpGet]
        [Route("EconomicMacros/{stateBudgetId}")]
        public async Task<EngineResponse> GetEconomicMacros(string stateBudgetId)
        {
            StateBudget stateBudget;
            if (string.IsNullOrEmpty(stateBudgetId))
            {
                stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
                if (stateBudget is null)
                {
                    throw new ServiceUnavailableException(
                        "Belum ada data APBN yang tersedia untuk saat ini.",
                        EngineErrorCode.NoStateBudgetData);
                }
            }
            else
            {
                if (!Guid.TryParse(stateBudgetId, out var stateBudgetGuid))
                {
                    throw new BadRequestException(
                        "Format identitas APBN tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }

                stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(stateBudgetGuid);
                if (stateBudget is null)
                {
                    throw new NotFoundException(
                        "Data APBN tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
            }

            var economicMacros = await UnitOfWork.EconomicMacros
                .GetByStateBudgetAsync(stateBudget);
            return new EngineResponse(economicMacros);
        }

        [HttpGet]
        [Route("SpecialPolicies")]
        public Task<EngineResponse> GetSpecialPolicies()
        {
            return GetSpecialPolicies(string.Empty);
        }
        
        [HttpGet]
        [Route("SpecialPolicies/{stateBudgetId}")]
        public async Task<EngineResponse> GetSpecialPolicies(string stateBudgetId)
        {
            StateBudget stateBudget;
            if (string.IsNullOrEmpty(stateBudgetId))
            {
                stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
                if (stateBudget is null)
                {
                    throw new ServiceUnavailableException(
                        "Belum ada data APBN yang tersedia untuk saat ini.",
                        EngineErrorCode.NoStateBudgetData);
                }
            }
            else
            {
                if (!Guid.TryParse(stateBudgetId, out var stateBudgetGuid))
                {
                    throw new BadRequestException(
                        "Format identitas APBN tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }
                
                stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(stateBudgetGuid);
                if (stateBudget is null)
                {
                    throw new NotFoundException(
                        "Data APBN tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
            }
            
            var specialPolicies = await UnitOfWork.SpecialPolicies
                .GetByStateBudgetAsync(stateBudget);
            return new EngineResponse(specialPolicies);
        }
        
        [HttpGet]
        [Route("SpecialPolicyAllocations/{specialPolicyId}")]
        public async Task<EngineResponse> GetSpecialPolicyAllocations(string specialPolicyId)
        {
            if (string.IsNullOrEmpty(specialPolicyId))
            {
                throw new BadRequestException(
                    "Identitas Kebijakan Khusus diperlukan, namun tidak ada dalam parameter.",
                    EngineErrorCode.RequiredDataNotProvided);
            }
            
            if (!Guid.TryParse(specialPolicyId, out var specialPolicyGuid))
            {
                throw new BadRequestException(
                    "Format identitas Kebijakan Khusus tidak valid.",
                    EngineErrorCode.InvalidDataFormat);
            }
                
            var specialPolicy = await UnitOfWork.SpecialPolicies.GetByIdAsync(specialPolicyGuid);
            if (specialPolicy is null)
            {
                throw new NotFoundException(
                    "Data Kebijakan Khusus tersebut tidak ditemukan.",
                    EngineErrorCode.DataNotFound);
            }
            
            var specialPolicyAllocations = await UnitOfWork.SpecialPolicyAllocations
                .FindAsync(allocation => allocation.SpecialPolicyId == specialPolicyGuid);
            return new EngineResponse(specialPolicyAllocations);
        }
        
        [HttpGet]
        [Route("StateBudgets")]
        public async Task<EngineResponse> GetStateBudgets()
        {
            var stateBudgets = await UnitOfWork.StateBudgets.GetAllAsync();
            return new EngineResponse(stateBudgets);
        }
        
        [HttpGet]
        [Route("StateBudgets/Active")]
        public async Task<EngineResponse> GetStateBudgetCurrent()
        {
            var stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
            return new EngineResponse(stateBudget);
        }

        [HttpGet]
        [Route("StateExpenditures")]
        public Task<EngineResponse> GetStateExpenditures()
        {
            return GetStateExpenditures(string.Empty);
        }
        
        [HttpGet]
        [Route("StateExpenditures/{stateBudgetId}")]
        public async Task<EngineResponse> GetStateExpenditures(string stateBudgetId)
        {
            StateBudget stateBudget;
            if (string.IsNullOrEmpty(stateBudgetId))
            {
                stateBudget = await UnitOfWork.StateBudgets.GetActiveAsync();
                if (stateBudget is null)
                {
                    throw new ServiceUnavailableException(
                        "Belum ada data APBN yang tersedia untuk saat ini.",
                        EngineErrorCode.NoStateBudgetData);
                }
            }
            else
            {
                if (!Guid.TryParse(stateBudgetId, out var stateBudgetGuid))
                {
                    throw new BadRequestException(
                        "Format identitas APBN tidak valid.",
                        EngineErrorCode.InvalidDataFormat);
                }
                
                stateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(stateBudgetGuid);
                if (stateBudget is null)
                {
                    throw new NotFoundException(
                        "Data APBN tersebut tidak ditemukan.",
                        EngineErrorCode.DataNotFound);
                }
            }
            
            var stateExpenditures = await UnitOfWork.StateExpenditures
                .GetByStateBudgetAsync(stateBudget);
            return new EngineResponse(stateExpenditures);
        }
        
        [HttpGet]
        [Route("StateExpenditureAllocations/{stateExpenditureId}")]
        public async Task<EngineResponse> GetStateExpenditureAllocations(string stateExpenditureId)
        {
            if (string.IsNullOrEmpty(stateExpenditureId))
            {
                throw new BadRequestException(
                    "Identitas Belanja Negara diperlukan, namun tidak ada dalam parameter.",
                    EngineErrorCode.RequiredDataNotProvided);
            }
            
            if (!Guid.TryParse(stateExpenditureId, out var stateExpenditureGuid))
            {
                throw new BadRequestException(
                    "Format identitas Belanja Negara tidak valid.",
                    EngineErrorCode.InvalidDataFormat);
            }
                
            var stateExpenditure = await UnitOfWork.StateExpenditures.GetByIdAsync(stateExpenditureGuid);
            if (stateExpenditure is null)
            {
                throw new NotFoundException(
                    "Data Kebijakan Khusus tersebut tidak ditemukan.",
                    EngineErrorCode.DataNotFound);
            }
            
            var stateExpenditureAllocations = await UnitOfWork
                .StateExpenditureAllocations
                .FindAsync(allocation => allocation.StateExpenditureId == stateExpenditureGuid);
            return new EngineResponse(stateExpenditureAllocations);
        }
        
        [HttpGet]
        [Route("Logs")]
        public async Task<FileResult> GetLogs()
        {
            var claimList = HttpContext.User.Claims.ToList();
            var authenticatedAccount = claimList.ToAccount();
            authenticatedAccount = await UnitOfWork.Accounts.GetByIdAsync(authenticatedAccount.Id);
            if (authenticatedAccount is null || authenticatedAccount.Role != AccountRole.DeveloperSupport)
            {
                throw new UnauthorizedException("Anda tidak memiliki kewenangan untuk mengakses sumber daya ini.", 
                    EngineErrorCode.NotAuthenticated);
            }
            
            var applicationName = typeof(DashboardController).Assembly.GetName().Name ?? "SimulasiAPBN";
            var folderName = $"Logs-{applicationName}";
            var logsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            var logFilePaths = Directory.GetFiles(logsFolderPath);

            await using var memoryStream = new MemoryStream();
            {
                using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true, Encoding.UTF8);
                {
                    foreach (var logFilePath in logFilePaths)
                    {
                        var logFileName = Path.GetFileName(logFilePath);
                        var zipArchiveEntry = zipArchive.CreateEntry(Path.Combine(folderName, logFileName));
                        await using var zipStream = zipArchiveEntry.Open();
                        {
                            await System.IO.File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                                .CopyToAsync(zipStream);
                        }
                    }
                    
                    return File(memoryStream.GetBuffer(), "application/zip", 
                        $"{folderName}.zip");
                }
            }
        }
        
        [HttpGet]
        [Route("SignInSessions")]
        public async Task<EngineResponse> GetSignInSessions()
        {
            var claimList = HttpContext.User.Claims.ToList();
            var authenticatedAccount = claimList.ToAccount();
            authenticatedAccount = await UnitOfWork.Accounts.GetByIdAsync(authenticatedAccount.Id);
            if (authenticatedAccount is null || authenticatedAccount.Role != AccountRole.DeveloperSupport)
            {
                throw new UnauthorizedException("Anda tidak memiliki kewenangan untuk mengakses sumber daya ini.", 
                    EngineErrorCode.NotAuthenticated);
            }

            var accounts = new Collection<object>();
            foreach (var account in await UnitOfWork.Accounts.GetAllAsync())
            {
                account.Password = null;

                var signInSessions = await UnitOfWork.SignInSessions.GetAllAsync();
                var signInAttempts = (await UnitOfWork.SignInAttempts
                        .FindAsync(entity => entity.AccountId == account.Id))
                    .OrderByDescending(entity => entity.CreatedAt)
                    .Select(signInAttempt =>
                    {
                        var signInSession = signInSessions
                            .FirstOrDefault(entity => entity.SignInAttemptId == signInAttempt.Id);
                        return new
                        {
                            signInAttempt.IpAddress,
                            signInAttempt.UserAgent,
                            signInAttempt.IsSuccess,
                            StatusCode = signInAttempt.StatusCode.ToString(),
                            IsRevoked = signInSession?.IsRevoked ?? false,
                            signInAttempt.CreatedAt
                        };
                    })
                    .ToList();
                
                accounts.Add(new
                {
                    account.Id,
                    account.Name,
                    account.Email,
                    account.Username,
                    account.Role,
                    account.IsActivated,
                    SignInAttempts = signInAttempts
                });
            }

            return new EngineResponse(accounts);
        }
    }
}