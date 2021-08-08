/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Pages.Dashboard.Policy
{
    public class EconomicMacroDetail : BasePage
    {

        public EconomicMacroDetail(IConfiguration configuration, IMemoryCache memoryCache, IUnitOfWork unitOfWork) 
            : base(configuration, memoryCache, unitOfWork)
        {
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
        }
        
        [BindProperty(SupportsGet = true)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public string EconomicMacroId { get; set; }
        
        public string EconomicMacroName { get; set; }
        public Core.Models.EconomicMacro EconomicMacro { get; private set; }
        public const string DescriptionIdentifier = "Deskripsi";
        public const string NarationIdentifier = "Kesimpulan Positif";
        public const string NarationMinusIdentifier = "Kesimpulan Negatif";

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();

            // Initialise variables with default value
            EconomicMacroName = "Asumsi";
            EconomicMacro = new Core.Models.EconomicMacro();
            
            // Populate based on facts
            if (string.IsNullOrEmpty(EconomicMacroId))
            {
                throw new BadRequestException("Mohon pilih Asumsi Ekonomi.");
            }
            
            if (!Guid.TryParse(EconomicMacroId, out var economicMacroGuid))
            {
                throw new BadRequestException("Data Asumsi tersebut tidak valid.");
            }

            EconomicMacro = await UnitOfWork.EconomicMacros.GetByIdAsync(economicMacroGuid);
            if (EconomicMacro is null)
            {
                EconomicMacro = new Core.Models.EconomicMacro();
                throw new BadRequestException("Data Asumsi Ekonomi tersebut tidak ditemukan.");
            }

            EconomicMacroName = $"Asumsi {EconomicMacro.Name}";
        }

        public async Task OnGet()
        {
            try
            {
                await Initialize();
            }
            catch (Exception e)
            {
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        public async Task OnPost()
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();

                foreach (var (key, values) in Request.Form)
                {
                    if (key == DescriptionIdentifier)
                    {
                        await SaveDescription(values.ToString());
                        continue;
                    }

                    if (key == NarationIdentifier)
                    {
                        await SaveNaration(values.ToString());
                        continue;
                    }
                    if (key == NarationMinusIdentifier)
                    {
                        await SaveNarationMinus(values.ToString());
                        continue;
                    }

                    if (!int.TryParse(key, out var index))
                    {
                        continue;
                    }
                }

                await Initialize();
                await UnitOfWork.CommitAsync();
                
                SetSuccessAlert($"Detail {EconomicMacroName} telah disimpan.");
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        private async Task SaveDescription(string description)
        {
            EconomicMacro.Description = description;
            await UnitOfWork.EconomicMacros.ModifyAsync(EconomicMacro);
        }
        private async Task SaveNaration(string naration)
        {
            EconomicMacro.Naration = naration;
            await UnitOfWork.EconomicMacros.ModifyAsync(EconomicMacro);
        }

        private async Task SaveNarationMinus(string naration)
        {
            EconomicMacro.NarationDefisit = naration;
            await UnitOfWork.EconomicMacros.ModifyAsync(EconomicMacro);
        }


    }
}