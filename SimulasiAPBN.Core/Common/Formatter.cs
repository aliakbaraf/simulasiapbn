/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Core.Common
{
    public static class Formatter
    {
        public static string GetAccountRoleName(AccountRole role)
        {
            return role switch
            {
                AccountRole.Unassigned => "Tidak Memiliki Peran",
                AccountRole.DeveloperSupport => "Dukungan Pengembang",
                AccountRole.Administrator => "Pengelola",
                AccountRole.Analyst => "Analis",
                _ => "Peran Lain (tidak dikenal)"
            };
        }
        
        public static string GetBudgetTypeName(BudgetType type)
        {
            return type switch
            {
                BudgetType.CentralGovernmentExpenditure => "Belanja Pemerintah Pusat",
                BudgetType.TransferToRegionalGovernment => "Transfer ke Daerah",
                _ => "Tidak diketahui"
            };
        }
        
        public static string GetStateBudgetPolicyName(StateBudget stateBudget)
        {
            if (stateBudget is null)
            {
                return string.Empty;
            }
            var name = $"APBN Tahun {stateBudget.Year}";
            if (stateBudget.Revision > 0)
            {
                name = string.Concat(name, $" Perubahan Ke-{stateBudget.Revision}");
            }

            return name;
        }

        public static string GetWebContentName(WebContentKey key)
        {
            return key switch
            {
                WebContentKey.Title => "Nama Aplikasi",
                WebContentKey.LandingText => "Kalimat Landing",
                WebContentKey.VideoUrl => "Video",
                WebContentKey.InvitationText => "Kalimat Ajakan",
                WebContentKey.HashTag => "Tagar",
                WebContentKey.Disclaimer => "Disclaimer",
                _ => ""
            };
        }
    }
}