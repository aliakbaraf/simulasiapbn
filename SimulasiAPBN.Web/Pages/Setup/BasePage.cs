/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Common.Configuration;

namespace SimulasiAPBN.Web.Pages.Setup
{
    public class BasePage : PageModel
    {
        protected readonly IConfiguration Configuration;

        public BasePage(IConfiguration configuration)
        {
            Configuration = configuration;
            GoogleTagManagerId = GoogleTagManager.GetId(Configuration);
        }

        public string GoogleTagManagerId { get; set; }
    }
}
