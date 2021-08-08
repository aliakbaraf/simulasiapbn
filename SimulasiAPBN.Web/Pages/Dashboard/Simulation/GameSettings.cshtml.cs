/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Common.Serializer;
using SimulasiAPBN.Core.Common;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Common.Extensions;

namespace SimulasiAPBN.Web.Pages.Dashboard.Simulation
{
    [RequestSizeLimit(52428800)]
    public class GameSettings : BasePage
    {
        private readonly IWebHostEnvironment _env;

        public GameSettings(
            IConfiguration configuration, 
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork, 
            IWebHostEnvironment env) 
            : base(configuration, memoryCache, unitOfWork)
        {
            _env = env;
            AllowedRoles = new List<AccountRole>
            {
                AccountRole.Administrator
            };
        }

        public string DeficitLaw { get; private set; }
        public decimal DeficitThreshold { get; private set; }
        public decimal DebtRatio { get; private set; }
        public decimal GrossDomesticProduct { get; private set; }
        [BindProperty]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global MemberCanBePrivate.Global
        public IFormFile VideoFormFile { get; set; }
        public IEnumerable<WebContent> WebContents { get; private set; }

        public const string SaveContentsAction = "SaveContentsAction";
        public const string SavePoliciesAction = "SavePoliciesAction";


        protected override async Task Initialize()
        {
            // Initialize base
            await base.Initialize();
            
            // Initialise variables with default value
            DeficitLaw = string.Empty;
            DeficitThreshold = decimal.One;
            DebtRatio = decimal.One;
            GrossDomesticProduct = decimal.Zero;
            WebContents = new List<WebContent>();
            
            // Populate based on facts
            DeficitLaw = await UnitOfWork.SimulationConfigs.GetDeficitLawAsync();
            DeficitThreshold = await UnitOfWork.SimulationConfigs.GetDeficitThresholdAsync();
            DebtRatio = await UnitOfWork.SimulationConfigs.GetDebtRatioAsync();
            GrossDomesticProduct = await UnitOfWork.SimulationConfigs.GetGrossDomesticProductAsync();
            WebContents = await UnitOfWork.WebContents.GetAllAsync();
        }

        public static bool IsMultilineValue(WebContentKey key)
        {
            return key switch
            {
                WebContentKey.Title => false,
                WebContentKey.LandingText => true,
                WebContentKey.VideoUrl => false,
                WebContentKey.InvitationText => true,
                WebContentKey.HashTag => false,
                _ => false
            };
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

        public async Task OnPost([FromForm] string action)
        {
            UnitOfWork.BeginTransaction();
            try
            {
                await Initialize();
                if (action is null) return;
                switch (action)
                {
                    case {} a when a == SaveContentsAction:
                        await SaveContents();
                        break;
                    case {} a when a == SavePoliciesAction:
                        await SavePolicies();
                        break;
                }
            }
            catch (Exception e)
            {
                await UnitOfWork.RollbackAsync();
                SetErrorAlert(e.Message);
                if (!(e is GenericException)) throw;
            }
        }

        private async Task SaveContents()
        {
            // Detect content values except for Video URL
            var webContentValues = new Dictionary<WebContentKey, string>();
            foreach (var webContent in WebContents)
            {
                if (webContent.Key == WebContentKey.VideoUrl)
                {
                    continue;
                }
                
                var value = Request.Form[Formatter.GetWebContentName(webContent.Key)].ToString();
                if (string.IsNullOrEmpty(value))
                {
                    throw new BadRequestException(
                        $"{Formatter.GetWebContentName(webContent.Key)} tidak boleh kosong.");
                }

                var webContentValue = IsMultilineValue(webContent.Key)
                    ? Json.Serialize(value.Split("\n"))
                    : value;
                webContentValues.Add(webContent.Key, webContentValue);
            }
            
            // Detect video upload
            if (VideoFormFile is not null)
            {
                var videoUrl = await UploadVideo();
                webContentValues.Add(WebContentKey.VideoUrl, videoUrl);
            }

            // Save contents to the database
            var appTitle = AppTitle;
            foreach (var webContent in WebContents)
            {
                if (!webContentValues.TryGetValue(webContent.Key, out var webContentValue))
                {
                    continue;
                }
                
                webContent.Value = webContentValue;
                await UnitOfWork.WebContents.ModifyAsync(webContent);

                if (webContent.Key == WebContentKey.Title)
                {
                    appTitle = webContent.Value;
                }
            }

            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert("Pengaturan Konten telah disimpan.");

            if (appTitle != AppTitle)
            {
                MemoryCache.SetAppTitle(appTitle);
            }
        }

        private async Task SavePolicies()
        {
            if (string.IsNullOrEmpty(Request.Form["DeficitThreshold"].ToString()))
            {
                throw new BadRequestException("Nilai Ambang Batas Defisit tidak boleh kosong.");
            }
            if (string.IsNullOrEmpty(Request.Form["DeficitLaw"].ToString()))
            {
                throw new BadRequestException("Undang-Undang yang mengatur tentang Defisit tidak boleh kosong.");
            }
            if (string.IsNullOrEmpty(Request.Form["DebtRatio"].ToString()))
            {
                throw new BadRequestException("Nilai Rasio Utang terhadap PDB tidak boleh kosong.");
            }
            if (string.IsNullOrEmpty(Request.Form["GrossDomesticProduct"].ToString()))
            {
                throw new BadRequestException("Nilai Produk Domestik Bruto tidak boleh kosong.");
            }

            if (!decimal.TryParse(Request.Form["DeficitThreshold"].ToString(), out var deficitThreshold))
            {
                throw new BadRequestException(
                    $"Nilai Ambang Batas Defisit {Request.Form["DeficitThreshold"]} tidak valid.");
            }
            if (!decimal.TryParse(Request.Form["DebtRatio"].ToString(), out var debtRatio))
            {
                throw new BadRequestException(
                    $"Nilai Rasio Utang terhadap PDB {Request.Form["DebtRatio"]} tidak valid.");
            }
            if (!decimal.TryParse(Request.Form["GrossDomesticProduct"].ToString(), out var grossDomesticProduct))
            {
                throw new BadRequestException(
                    $"Nilai Produk Domestik Bruto {Request.Form["GrossDomesticProduct"]} tidak valid.");
            }
            var deficitLaw = Request.Form["DeficitLaw"].ToString();

            await UnitOfWork.SimulationConfigs.SetDeficitThresholdAsync(deficitThreshold);
            await UnitOfWork.SimulationConfigs.SetDeficitLawAsync(deficitLaw);
            await UnitOfWork.SimulationConfigs.SetDebtRatioAsync(debtRatio);
            await UnitOfWork.SimulationConfigs.SetGrossDomesticProductAsync(grossDomesticProduct);
            
            await Initialize();
            await UnitOfWork.CommitAsync();
            SetSuccessAlert("Pengaturan Kebijakan Terkait telah disimpan.");
        }

        private async Task<string> UploadVideo()
        {
            if (VideoFormFile is null)
            {
                return "";
            }

            if (VideoFormFile.Length > 41943040)
            {
                throw new BadRequestException("Ukuran berkas video maksimum 40 MB.");
            }
        
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var videoDirectoryPath = Path.Combine(_env.WebRootPath, "media", timestamp.ToString());
            var videoFilePath = Path.Combine(videoDirectoryPath, VideoFormFile.FileName);
            var videoUrl = $"/media/{timestamp}/{VideoFormFile.FileName}";

            Directory.CreateDirectory(videoDirectoryPath);
            await using var fileStream = new FileStream(videoFilePath, FileMode.Create);
            await VideoFormFile.CopyToAsync(fileStream);

            return videoUrl;
        }
    }
}