/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using Microsoft.Extensions.Configuration;

namespace SimulasiAPBN.Web.Pages.Setup
{
    public class Index : BasePage
    {
        public Index(IConfiguration configuration) : base(configuration) { }

        public void OnGet()
        {
        }
    }
}