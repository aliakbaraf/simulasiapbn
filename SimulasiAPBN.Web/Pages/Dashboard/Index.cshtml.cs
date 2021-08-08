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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;

namespace SimulasiAPBN.Web.Pages.Dashboard
{
    public class Index : BasePage
    {
        public Index(IConfiguration configuration, IMemoryCache memoryCache, IUnitOfWork unitOfWork) 
            : base(configuration, memoryCache, unitOfWork)
        {
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator,
                AccountRole.Analyst
            };
        }

        [BindProperty(SupportsGet = true)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public string StateBudgetId { get; set; }

        public StateBudget ActiveStateBudget { get; private set; }
        public string ApplicationStatus { get; private set; }
        public string ApplicationStatusDescription { get; private set; }
        public string Greeting { get; private set; }
        public bool IsApplicationReady { get; private set; }
        public StateBudget LatestStateBudget { get; private set; }
        public StateBudget StateBudget { get; private set; }
        public ICollection<StateBudget> StateBudgets { get; private set; }
        public Dictionary<Budget, int> PriorityBudgets { get; private set; }
        public SimulationCount SimulationCount { get; private set; }
        public SimulationCount SimulationCountOnCurrentPolicy { get; private set; }
        public ShareCount ShareCount { get; private set; }
        public ShareClickCount ShareClickCount { get; private set; }

        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();

            // Initialise variables with default value
            var isActiveFromPreviousYear = false;

            ActiveStateBudget = new StateBudget();
            ApplicationStatus = "SIAP DIMAINKAN";
            ApplicationStatusDescription = "Permainan simulasi telah dikonfigurasi sepenuhnya dan siap dimainkan.";
            Greeting = string.Empty;
            IsApplicationReady = true;
            LatestStateBudget = new StateBudget();
            StateBudget = new StateBudget();
            StateBudgets = new Collection<StateBudget>();
            PriorityBudgets = new Dictionary<Budget, int>();
            SimulationCount = new SimulationCount(0, 0, 0);
            SimulationCountOnCurrentPolicy = new SimulationCount(0, 0, 0);
            ShareCount = new ShareCount(0, 0);
            ShareClickCount = new ShareClickCount(0, 0);
            
            // Populate based on facts
            Greeting = DateTimeOffset.Now.Hour switch
            {
                <= 10 => "Selamat Pagi",
                <= 15 => "Selamat Siang",
                <= 17 => "Selamat Petang",
                _ => "Selamat Malam"
            };

            
            StateBudgets = (await UnitOfWork.StateBudgets.GetAllAsync())
                .OrderBy(stateBudget => stateBudget.Year)
                .ThenBy(stateBudget => stateBudget.Revision)
                .ToList();
            ActiveStateBudget = UnitOfWork.StateBudgets.GetActiveFromList(StateBudgets);
            if (ActiveStateBudget is null)
            {
                ActiveStateBudget = UnitOfWork.StateBudgets
                    .GetActiveFromList(StateBudgets, DateTimeOffset.Now.Year - 1);
                isActiveFromPreviousYear = true;
            }
            LatestStateBudget = UnitOfWork.StateBudgets.GetLatestFromList(StateBudgets);
            StateBudget = LatestStateBudget;

            if (!StateBudgets.Any())
            {
                IsApplicationReady = false;
                ApplicationStatus = "TIDAK DAPAT DIMAINKAN";
                ApplicationStatusDescription = "Belum ada Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) " +
                    "yang ditambahkan. Silakan buka panel Kebijakan > APBN untuk menambahkan Kebijakan APBN.";
                return;
            }

            var currentYear = DateTimeOffset.Now.Year;
            var previousYear = currentYear - 1;
            if (!StateBudgets.Where(e => e.Year == currentYear || e.Year == previousYear).Any())
            {
                IsApplicationReady = false;
                ApplicationStatus = "TIDAK DAPAT DIMAINKAN";
                ApplicationStatusDescription = "Tidak ada Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) " +
                    $"untuk Tahun Anggaran {previousYear} dan {currentYear}. Silakan buka panel Kebijakan > APBN " +
                    "untuk menambahkan Kebijakan APBN.";
                return;
            }

            if (ActiveStateBudget is null)
            {
                var year = isActiveFromPreviousYear ? previousYear : currentYear;
                IsApplicationReady = false;
                ApplicationStatus = "TIDAK DAPAT DIMAINKAN";
                ApplicationStatusDescription = "Ditemukan Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) " +
                    $"untuk Tahun Anggaran {year}, namun tidak ada yang berlaku. Pastikan komponen Pendapatan " +
                    $"Negara dan Belanja Negara telah diatur untuk Kebijakan APBN Tahun Anggaran {year}.";
                return;
            }

            if (isActiveFromPreviousYear)
            {
                ApplicationStatus = "SIAP DIMAINKAN, NAMUN DENGAN CATATAN";
                ApplicationStatusDescription = "Tidak ada Kebijakan Anggaran Pendapatan dan Belanja Negara (APBN) " +
                    $"untuk Tahun Anggaran {currentYear}. Untuk sementara, permainan akan menggunakan data Kebijakan " +
                    $"APBN Tahun Anggaran {previousYear}. Silakan buka panel Kebijakan > APBN untuk menambahkan " +
                    "Kebijakan APBN.";
                return;
            }
            
            if (!string.IsNullOrEmpty(StateBudgetId))
            {
                if (!Guid.TryParse(StateBudgetId, out var stateBudgetGuid))
                {
                    throw new BadRequestException("Kebijakan APBN tidak valid.");
                }

                StateBudget = await UnitOfWork.StateBudgets.GetByIdAsync(stateBudgetGuid);
            }

            if (StateBudget is null)
            {
                StateBudget = new StateBudget();
                throw new NotFoundException("Data Kebijakan APBN yang Anda pilih tidak ditemukan.");
            }

            var simulationStateExpenditures = (await UnitOfWork
                    .SimulationStateExpenditures
                    .GetAllAsync())
                .ToList();
            var stateExpenditures = (await UnitOfWork.StateExpenditures
                    .GetByStateBudgetAsync(StateBudget))
                .ToList();
            foreach (var stateExpenditure in stateExpenditures)
            {
                var simulations = simulationStateExpenditures
                    .FindAll(e => e.StateExpenditureId == stateExpenditure.Id);
                PriorityBudgets.Add(stateExpenditure.Budget, simulations.Count(e => e.IsPriority));
            }

            var simulationSessions = (await UnitOfWork.SimulationSessions.GetAllAsync())
                .ToList();
            SimulationCount = new SimulationCount(
                simulationSessions.Count(e => e.SimulationState == SimulationState.Completed),
                simulationSessions.Count(e => e.SimulationState == SimulationState.OnProgress),
                simulationSessions.Count(e => e.SimulationState == SimulationState.Started)
                );

            var simulationSessionsOnCurrentPolicy = simulationSessions
                .FindAll(e => e.StateBudgetId == StateBudget.Id);
            SimulationCountOnCurrentPolicy = new SimulationCount(
                simulationSessionsOnCurrentPolicy
                    .Count(e => e.SimulationState == SimulationState.Completed),
                simulationSessionsOnCurrentPolicy
                    .Count(e => e.SimulationState == SimulationState.OnProgress),
                simulationSessionsOnCurrentPolicy
                    .Count(e => e.SimulationState == SimulationState.Started)
            );

            var simulationShares = (await UnitOfWork.SimulationShares.GetAllAsync())
                .ToList();
            ShareCount = new ShareCount(
                simulationShares.Count(e => e.Target == SimulationShareTarget.FacebookPost),
                simulationShares.Count(e => e.Target == SimulationShareTarget.TwitterTweet)
            );
            var facebookPostShares = simulationShares
                .FindAll(e => e.Target == SimulationShareTarget.FacebookPost);
            var twitterTweetShares = simulationShares
                .FindAll(e => e.Target == SimulationShareTarget.TwitterTweet);
            ShareClickCount = new ShareClickCount(
                facebookPostShares.Sum(e => e.ClickedTimes),
                twitterTweetShares.Sum(e => e.ClickedTimes)
            );
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
    }
    
    public class SimulationCount
    {
        public SimulationCount(int completed, int onProgress, int started)
        {
            Completed = completed;
            OnProgress = onProgress;
            Started = started;
            All = Completed + OnProgress + Started;
        }
        
        public int All { get; init; }
        public int Completed { get; init; }
        public int OnProgress { get; init; }
        public int Started { get; init; }
    }
    
    public class ShareCount
    {
        public ShareCount(int facebookPost, int twitterTweet)
        {
            FacebookPost = facebookPost;
            TwitterTweet = twitterTweet;
            All = FacebookPost + TwitterTweet;
        }
        
        public int All { get; init; }
        public int FacebookPost { get; init; }
        public int TwitterTweet { get; init; }
    }
    public class ShareClickCount
    {
        public ShareClickCount(int facebookPostClick, int twitterTweetClick)
        {
            FacebookPostClick = facebookPostClick;
            TwitterTweetClick = twitterTweetClick;
            All = FacebookPostClick + TwitterTweetClick;
        }
        
        public int All { get; init; }
        public int FacebookPostClick { get; init; }
        public int TwitterTweetClick { get; init; }
    }
}