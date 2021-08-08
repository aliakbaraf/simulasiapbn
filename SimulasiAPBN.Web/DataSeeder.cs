/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Information;
using SimulasiAPBN.Common.Serializer;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Infrastructure.EntityFrameworkCore;

namespace SimulasiAPBN.Web
{
    public class DataSeeder
    {
        private static readonly Guid EducationAllocationId = Guid.Parse("67261940-8200-4912-810a-b12070b9ff0e");
        private static readonly Guid HealthAllocationId = Guid.Parse("9fef3f45-bd5e-4f40-9db5-b14126047203");
        
        private static readonly Dictionary<Guid, decimal> Expenditures = new();
        private static readonly Dictionary<Guid, decimal> EducationExpenditures = new();
        private static readonly Dictionary<Guid, decimal> HealthExpenditures = new();

        private static Allocation CreateAllocationIfNotExists(
            IUnitOfWork unitOfWork, 
            string name)
        {
            var allocation = new Allocation
            {
                Name = name,
                IsMandatory = false
            };
            unitOfWork.Allocations.Add(allocation);
            return allocation;
        }
        
        private static Allocation CreateAllocationIfNotExists(
            IUnitOfWork unitOfWork, 
            Guid id,
            string name, 
            string description,
            decimal threshold)
        {
            var allocation = unitOfWork.Allocations.GetById(id);
            if (allocation is not null) return allocation;
            allocation = new Allocation
            {
                Id = id,
                Name = name,
                IsMandatory = false,
                MandatoryExplanation = description,
                MandatoryThreshold = threshold,
            };
            unitOfWork.Allocations.Add(allocation);
            return allocation;
        }

        private static Budget CreateBudgetIfNotExists(
            IUnitOfWork unitOfWork, 
            string function,
            BudgetType type, 
            string description, 
            IEnumerable<string> targets)
        {
            var budget = unitOfWork.Budgets
                .FindOne(entity => entity.Function == function && entity.Type == type);
            if (budget is not null) return budget;
            budget = new Budget
            {
                Function = function,
                Type = type,
                Description = description
            };
            unitOfWork.Budgets.Add(budget);
            var budgetTargets = targets.ToList()
                .Select(target => new BudgetTarget
                {
                    BudgetId = budget.Id, 
                    Description = target
                }).ToList();
            unitOfWork.BudgetTargets.AddRange(budgetTargets);
            return budget;
        }

        private static StateBudget CreateStateBudgetIfNotExists(
            IUnitOfWork unitOfWork, int year, int revision, decimal countryIncome)
        {
            var stateBudget = unitOfWork.StateBudgets
                .FindOne(entity =>  entity.Year == year && entity.Revision == revision);
            if (stateBudget is not null) return stateBudget;
            stateBudget = new StateBudget
            {
                Year = year, 
                Revision = revision,
                CountryIncome = countryIncome
            };
            unitOfWork.StateBudgets.Add(stateBudget);
            return stateBudget;
        }

        private static EconomicMacro CreateEconomicMacroIfNotExists(
            IUnitOfWork unitOfWork,
            StateBudget stateBudget,
            string name,
            string description,
            string unitDesc,
            int orderFlag,
            decimal defaultValue,
            decimal threshold,
            decimal thresholdValue,
            decimal minValue,
            decimal maxValue)
        {
            var economicMacro = unitOfWork.EconomicMacros
                .FindOne(entity => entity.Name == name);
            if (economicMacro is not null) return economicMacro;
            economicMacro = new EconomicMacro
            {
                Name = name,
                StateBudgetId = stateBudget.Id,
                Description = description,
                DefaultValue = defaultValue,
                Threshold = threshold,
                ThresholdValue = thresholdValue,
                MinimumValue = minValue,
                MaximumValue = maxValue,
                OrderFlag = orderFlag,
                UnitDesc = unitDesc,
            };
            unitOfWork.EconomicMacros.Add(economicMacro);
            return economicMacro;
        }


        private static StateExpenditure CreateStateExpenditureIfNotExists(
            IUnitOfWork unitOfWork, StateBudget stateBudget, Budget budget, decimal totalAllocation)
        {
            var stateExpenditure = unitOfWork.StateExpenditures
                .FindOne(entity => entity.StateBudget == stateBudget && entity.Budget == budget);
            if (stateExpenditure is not null) return stateExpenditure;
            stateExpenditure = new StateExpenditure
            {
                StateBudgetId = stateBudget.Id,
                StateBudget = stateBudget,
                BudgetId = budget.Id,
                Budget = budget,
                SimulationMaximumMultiplier = 2.00m,
                TotalAllocation = totalAllocation,
                StateExpenditureAllocations = new List<StateExpenditureAllocation>()
            };
            unitOfWork.StateExpenditures.Add(stateExpenditure);
            return stateExpenditure;
        }

        private static void CreateStateExpenditureAllocationIfNotExists(IUnitOfWork unitOfWork,
            StateExpenditure stateExpenditure,
            Allocation allocation,
            decimal totalAllocation)
        {
            var stateExpenditureAllocation = unitOfWork.StateExpenditureAllocations
                .FindOne(entity =>
                    entity.Allocation == allocation && 
                    entity.StateExpenditure == stateExpenditure);
            if (stateExpenditureAllocation is not null) return;
            
            var percentage = (totalAllocation / stateExpenditure.TotalAllocation) * 100;
            stateExpenditureAllocation = new StateExpenditureAllocation
            {
                Allocation = allocation, 
                AllocationId = allocation.Id,
                Percentage = percentage,
                StateExpenditure = stateExpenditure,
                StateExpenditureId = stateExpenditure.Id,
                TotalAllocation = totalAllocation
            };
            unitOfWork.StateExpenditureAllocations.Add(stateExpenditureAllocation);
        }
        
        
        private static SpecialPolicy CreateSpecialPolicyIfNotExists(
            IUnitOfWork unitOfWork, StateBudget stateBudget, string name, string description, decimal totalAllocation)
        {
            var specialPolicy = unitOfWork.SpecialPolicies
                .FindOne(entity => 
                    entity.StateBudgetId == stateBudget.Id && entity.Name == name && entity.IsActive);
            if (specialPolicy is not null) return specialPolicy;
            specialPolicy = new SpecialPolicy
            {
                Name = name,
                Description = description,
                StateBudgetId = stateBudget.Id,
                IsActive = true,
                TotalAllocation = totalAllocation,
                SpecialPolicyAllocations = new List<SpecialPolicyAllocation>()
            };
            unitOfWork.SpecialPolicies.Add(specialPolicy);
            return specialPolicy;
        }
        
        private static void CreateSpecialPolicyAllocationIfNotExists(IUnitOfWork unitOfWork,
            SpecialPolicy specialPolicy,
            Allocation allocation,
            decimal totalAllocation)
        {
            var specialPolicyAllocation = unitOfWork.SpecialPolicyAllocations
                .FindOne(entity =>
                    entity.Allocation == allocation && entity.SpecialPolicyId == specialPolicy.Id);
            if (specialPolicyAllocation is not null) return;
            var percentage = (totalAllocation / specialPolicy.TotalAllocation) * 100;
            specialPolicyAllocation = new SpecialPolicyAllocation
            {
                AllocationId = allocation.Id, 
                Allocation = allocation,
                Percentage = percentage,
                SpecialPolicyId = specialPolicy.Id,
                TotalAllocation = totalAllocation,
                SimulationMaximumMultiplier = 2.0m,
            };
            unitOfWork.SpecialPolicyAllocations.Add(specialPolicyAllocation);
        }
        
        private static SimulationConfig CreateSimulationConfigIfNotExists(
            IUnitOfWork unitOfWork, SimulationConfigKey simulationConfigKey, string simulationConfigValue)
        {
            var simulationConfig = unitOfWork.SimulationConfigs
                .FindOne(entity => entity.Key == simulationConfigKey);
            if (simulationConfig is not null) return simulationConfig;
            simulationConfig = new SimulationConfig
            {
                Key = simulationConfigKey,
                Value = simulationConfigValue
            };
            unitOfWork.SimulationConfigs.Add(simulationConfig);
            return simulationConfig;
        }
        
        private static void CreateWebContentIfNotExists(IUnitOfWork unitOfWork, WebContentKey webContentKey,
            string webContentValue)
        {
            var webContent = unitOfWork.WebContents
                .FindOne(entity => entity.Key == webContentKey);
            if (webContent is not null) return;
            webContent = new WebContent
            {
                Key = webContentKey,
                Value = webContentValue
            };
            unitOfWork.WebContents.Add(webContent);
        }

        private static void InitializeAccount(IUnitOfWork unitOfWork)
        {
            var account = unitOfWork.Accounts.GetById(Developer.Id);
            if (account is not null) return;
            account = new Account
            {
                Id = Developer.Id,
                Name = Developer.Name,
                Email = Developer.Email,
                Username = Developer.Username,
                Password = string.Empty,
                IsActivated = true,
                Role = AccountRole.DeveloperSupport
            };
            unitOfWork.Accounts.Add(account);
        }
        
        private static IEnumerable<Allocation> InitializeAllocation(IUnitOfWork unitOfWork)
        {
            var educationAllocation = CreateAllocationIfNotExists(unitOfWork,
                EducationAllocationId,
                "Pendidikan",
                "Sesuai dengan amanat pasal 31 ayat 4 Undang-Undang Dasar 1945, Negara " +
                "memrioritaskan Anggaran Pendidikan sekurang-kurangnya 20% dari APBN untuk memenuhi " +
                "kebutuhan penyelenggaraan pendidikan nasional.",
                20
            );
            var healthAllocation = CreateAllocationIfNotExists(unitOfWork,
                HealthAllocationId,
                "Kesehatan",
                "Sesuai dengan amanat pasal 171 Undang-Undang Nomor 36 Tahun 2009 tentang " +
                "Kesehatan, besar Anggaran Kesehatan yand dialokasikan pemerintah minimum sebesar 5% " +
                "dari APBN di luar gaji.",
                5
            );
            var infrastructureAllocation = CreateAllocationIfNotExists(unitOfWork, "Insfrastruktur");
            return new List<Allocation> { educationAllocation, healthAllocation, infrastructureAllocation };
        }

        private static IEnumerable<EconomicMacro> InitializeEconomicMacro(IUnitOfWork unitOfWork, StateBudget stateBudget)
        {
            var economicMacros = new List<EconomicMacro>();
            var economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Pertumbuhan Ekonomi",
                "Pertumbuhan Ekonomi description",
                "%",
                1,
                5m,
                0.1m,
                1263.4m,
                3m,
                8m
            );
            economicMacros.Add(economicMacro);

            economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Inflasi",
                "description",
                "%",
                2,
                3m,
                0.1m,
                1287.9m,
                1m,
                5m
            );
            economicMacros.Add(economicMacro);

            economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Nilai Tukar",
                "description",
                "Rp/USD1",
                3,
                14600m,
                100m,
                1287.9m,
                10000m,
                20000m
            );
            economicMacros.Add(economicMacro);

            economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Crude-Oil Price",
                "description",
                "US$/Barrel",
                4,
                45m,
                1m,
                2837.1m,
                30m,
                60m
            );
            economicMacros.Add(economicMacro);

            economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Lifting Migas",
                "description",
                "ribu barel/hari",
                5,
                705m,
                10m,
                705m,
                500m,
                1000m
            );
            economicMacros.Add(economicMacro);

            economicMacro = CreateEconomicMacroIfNotExists(unitOfWork, stateBudget,
                "Suku Bunga SBN 10 tahun",
                "description",
                "%",
                6,
                7.29m,
                0.01m,
                0m,
                5m,
                10m
            );
            economicMacros.Add(economicMacro);


            return economicMacros;
        }

        private static IEnumerable<Budget> InitializeBudget(IUnitOfWork unitOfWork)
        {
            var budgets = new List<Budget>();
            
            var budget = CreateBudgetIfNotExists(unitOfWork,
                "Perumahan dan Fasilitas Umum", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 33.2173m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Pelayanan Umum", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 526.1813m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Ketertiban dan Keamanan", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 166.6322m);
            EducationExpenditures.Add(budget.Id, 0.5m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Ekonomi", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 511.3381m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Perlindungan Lingkungan Hidup", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 16.6899m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Pertahanan", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 137.1856m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Kesehatan",
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 111.6667m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Pariwisata", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 5.2614m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Agama", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 11.0759m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Pendidikan", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 175.2365m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Perlindungan Sosial", 
                BudgetType.CentralGovernmentExpenditure, 
                "", new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 260.0636m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Dana Transfer Umum", 
                BudgetType.TransferToRegionalGovernment, 
                "Dana Transfer Umum terdiri atas Dana Alokasi Umum (DAU) dan Dana Bagi Hasil (DBH) yang penggunaannya menjadi kewenangan daerah. " +
                "Daerah mempunyai diskresi untuk menggunakan Dana Transfer Umum sesuai dengan kebutuhan dan prioritas daerah. " +
                "Untuk mempercepat pembangunan insfrastruktur yang berorientasi pada peningkatan kuantitas dan kualitas layanan publik, maka diatur minimal 25 persen dari Dana Transfer Umum digunakan untuk belanja Insfrastruktur.",
                new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 492.253011279m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Dana Transfer Khusus", 
                BudgetType.TransferToRegionalGovernment, 
                "Dana Transfer Khusus terdiri dari Dana Alokasi Khusus (DAK) Fisik dan Dana Alokasi Khusus (DAK) Non-Fisik. " +
                "DAK fisik untuk mendanai insfraktruktur dan sarana/prasarana pelayanan publik dan penunjang kegiatan ekonomi yang menjadi kewenangan daerah. " +
                "DAK nonfisik untuk aksesibilitas masyarakat terhadap layanan dasar publik. Contoh: dana Bantuan Operasional Sekolah (BOS), dana Bantuan Operasional Penyelenggaraan Pendidikan Anak Usia Dini (BOP PAUD), dana Tunjangan Profesi Guru PNSD, dana Tambahan Penghasilan Guru PNSD, dana Tunjangan Khusus Guru PNSD di daerah khusus, dana Bantuan Operasional Kesehatan (BOK), dana Bantuan Operasional Keluarga Berencana (BOKB), dana Peningkatan Kapasitas Koperasi, Usaha Kecil dan Menengah (PK2UKM), dana Pelayanan Administrasi Kependudukan, dan empat jenis inisiatif baru DAK Nonfisik, dana Bantuan Operasional Penyelenggaraan (BOP) Pendidikan Kesetaraan, dana Bantuan Operasional Penyelenggaraan (BOP) Museum dan Taman Budaya, dana Pelayanan Kepariwisataan, dan dana Bantuan Biaya Layanan Pengolahan Sampah (BLPS).", 
                new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 196.423545m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Dana Insentif Daerah", 
                BudgetType.TransferToRegionalGovernment, 
                "Dana Insentif Daerah adalah dana yang dialokasikan kepada daerah tertentu berdasarkan kategori/kriteria tertentu sebagai penghargaan atas perbaikan dan/atau pencapaian kinerja bidang pengelolaan keuangan daerah, pelayanan dasar publik, dan kesejahteraan masyarakat.", 
                new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 13.5m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            budget = CreateBudgetIfNotExists(unitOfWork,
                "Dana Otonomi Khusus dan Dana Keistimewaan DIY",
                BudgetType.TransferToRegionalGovernment, 
                "Dana Otonomi Khusus dialokasikan untuk mendukung pelaksanaan Undang-Undang mengenai otonomi khusus baik di Provinsi Aceh, Provinsi Papua dan Provinsi Papua Barat. " +
                "Dana Keistimewaan Daerah Istimewa Yogyakarta adalah dana yang dialokasikan untuk mendukung pembiayaan bagi penyelenggaraan kewenangan keistimewaan DIY sesuai dengan Undang-Undang Nomor 13 tahun 2012 tentang Keistimewaan DIY.", 
                new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 21.302919182m);
            EducationExpenditures.Add(budget.Id, 4m);
            HealthExpenditures.Add(budget.Id, 0m);
            
            budget = CreateBudgetIfNotExists(unitOfWork,
                "Dana Desa", 
                BudgetType.TransferToRegionalGovernment, 
                "Alokasi ini diperuntukan bagi desa, yang di transfer melalui anggaran belanja daerah kabupaten/kota, untuk membiayai penyelenggaraan pemerintah, pelaksanaan pembangunan, pembinaan kemasyarakatan, dan pemberdaya masyarakat desa.", 
                new List<string>());
            budgets.Add(budget);
            Expenditures.Add(budget.Id, 72m);
            EducationExpenditures.Add(budget.Id, 0m);
            HealthExpenditures.Add(budget.Id, 0m);

            return budgets;
        }

        private static IEnumerable<StateExpenditure> InitializeStateExpenditure(
            IUnitOfWork unitOfWork, StateBudget stateBudget, IEnumerable<Budget> budgets)
        {
            return budgets.Select(budget =>
                    CreateStateExpenditureIfNotExists(
                        unitOfWork,
                        stateBudget,
                        budget,
                        Expenditures.GetValueOrDefault(budget.Id, 0m)));
        }
        
        private static void InitializeStateExpenditureAllocation(IUnitOfWork unitOfWork,
            StateExpenditure stateExpenditure, ICollection<Allocation> allocations)
        {
            var educationAllocation = allocations
                .FirstOrDefault(entity => entity.Id == EducationAllocationId);
            var healthAllocation = allocations
                .FirstOrDefault(entity => entity.Id == HealthAllocationId);
            
            CreateStateExpenditureAllocationIfNotExists(
                unitOfWork,
                stateExpenditure,
                educationAllocation,
                EducationExpenditures.GetValueOrDefault(stateExpenditure.Budget.Id, 0m));
            CreateStateExpenditureAllocationIfNotExists(
                unitOfWork,
                stateExpenditure,
                healthAllocation,
                HealthExpenditures.GetValueOrDefault(stateExpenditure.Budget.Id, 0m));
        }
        
        private static void InitializeSpecialPolicyAllocation(
            IUnitOfWork unitOfWork,
            SpecialPolicy specialPolicy,
            ICollection<Allocation> allocations)
        {
            var educationAllocation = allocations
                .FirstOrDefault(entity => entity.Id == EducationAllocationId);
            var healthAllocation = allocations
                .FirstOrDefault(entity => entity.Id == HealthAllocationId);

            CreateSpecialPolicyAllocationIfNotExists(unitOfWork,
                specialPolicy, educationAllocation, 1m);
            CreateSpecialPolicyAllocationIfNotExists(unitOfWork,
                specialPolicy, healthAllocation, 1m);
        }

        private static void InitializeSimulationConfig(IUnitOfWork unitOfWork)
        {
            CreateSimulationConfigIfNotExists(unitOfWork, SimulationConfigKey.IsAppInstalled,
                false.ToString());
            CreateSimulationConfigIfNotExists(unitOfWork, SimulationConfigKey.DeficitThreshold,
                $"{3.0m}");
            CreateSimulationConfigIfNotExists(unitOfWork, SimulationConfigKey.DeficitLaw,
                "Undang-Undang Nomor 17 Tahun 2003 tentang Keuangan Negara");
            CreateSimulationConfigIfNotExists(unitOfWork, SimulationConfigKey.DebtRatio,
                $"{60.0m}");
            CreateSimulationConfigIfNotExists(unitOfWork, SimulationConfigKey.GrossDomesticProduct,
                $"{16086.96739130435m}");
        }

        private static void InitializeWebContent(IUnitOfWork unitOfWork)
        {
            var landingTexts = new List<string>
            {
                "Saatnya kamu jadi Menteri Keuangan", 
                "Rancang APBN versi kamu sendiri!"
            };
            var invitationTexts = new List<string>
            {
                "Pernahkah kamu merancang APBN?",
                "Jika belum, kamu bisa merancang APBN",
                "melalui aplikasi Simulasi APBN.",
                "",
                "Ayo, rancang APBN versi kamu",
                "sekarang juga!"
            };

            CreateWebContentIfNotExists(unitOfWork, WebContentKey.Title,
                "Simulasi APBN");
            CreateWebContentIfNotExists(unitOfWork, WebContentKey.LandingText,
                Json.Serialize(landingTexts));
            CreateWebContentIfNotExists(unitOfWork, WebContentKey.VideoUrl,
                "/media/introduction.mp4");
            CreateWebContentIfNotExists(unitOfWork, WebContentKey.InvitationText,
                Json.Serialize(invitationTexts));
            CreateWebContentIfNotExists(unitOfWork, WebContentKey.HashTag,
                "#UangKita");
            CreateWebContentIfNotExists(unitOfWork, WebContentKey.Disclaimer,
                "Apapun yang kamu lakukan disini tidak menjadi acuan menjadi menteri keuangan");
        }

        private static void EnsureMigrated(DbContext dbContext)
        {
            dbContext.Database.Migrate();
            dbContext.SaveChanges();
        }
        
        public static bool Initialize(IServiceProvider serviceProvider)
        {
            var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var logger = serviceProvider.GetRequiredService<ILogger<DataSeeder>>();
            
            logger.LogInformation("Ensuring database is created...");
            EnsureMigrated(applicationDbContext);

            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            unitOfWork.BeginTransaction();

            try
            {
                var isDataSeedDoneConfig = CreateSimulationConfigIfNotExists(
                    unitOfWork, SimulationConfigKey.IsDataSeedDone,
                    false.ToString());

                if (isDataSeedDoneConfig.Value == true.ToString())
                {
                    unitOfWork.Commit();
                    logger.LogInformation("Database is ready");
                    return true;
                }

                logger.LogInformation("Starting data seeder...");

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.Account");
                InitializeAccount(unitOfWork);

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.SimulationConfig");
                InitializeSimulationConfig(unitOfWork);

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.WebContent");
                InitializeWebContent(unitOfWork);
                
                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.Allocation");
                var allocations = InitializeAllocation(unitOfWork).ToList();
                
                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.Budget");
                var budgets = InitializeBudget(unitOfWork);

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.StateBudget");
                var stateBudget = CreateStateBudgetIfNotExists(unitOfWork, 2021, 0, 1743.64854732m);

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.EconomicMacro");
                var economicMacro = InitializeEconomicMacro(unitOfWork, stateBudget);

                logger.LogInformation("Seeding SimulasiAPBN.Core.Models.StateExpenditure");
                var stateExpenditures = InitializeStateExpenditure(
                    unitOfWork, stateBudget, budgets);

                foreach (var stateExpenditure in stateExpenditures)
                {
                    InitializeStateExpenditureAllocation(unitOfWork, stateExpenditure, allocations);
                }
                
                if (!env.IsProduction())
                {
                    logger.LogInformation("Seeding SimulasiAPBN.Core.Models.SpecialPolicy");
                    var specialPolicy = CreateSpecialPolicyIfNotExists(unitOfWork, stateBudget,
                        "Percepatan Pemulihan Ekonomi dan Penguatan Reformasi",
                        "Program Percepatan Pemulihan Ekonomi dan Penguatan Reformasi",
                        10m);

                    logger.LogInformation("Seeding SimulasiAPBN.Core.Models.SpecialPolicyAllocation");
                    InitializeSpecialPolicyAllocation(unitOfWork, specialPolicy, allocations);

                }

                logger.LogInformation("Cleaning up seeder...");
                isDataSeedDoneConfig.Value = true.ToString();
                unitOfWork.SimulationConfigs.Modify(isDataSeedDoneConfig);

                unitOfWork.Commit();
                logger.LogInformation("Database is ready");
                return true;
            }
            catch (Exception exception)
            {
                unitOfWork.Rollback();
                logger.LogError(exception, "An error occured when seeding data");
                return false;
            }
        }
    }
}