/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;

namespace SimulasiAPBN.Common.Configuration
{
    public static class AntiForgery
    {
        public static bool CookieHttpOnly { get; } = true;
        public static string CookieName { get; } = "SimulasiAPBN.AntiForgery";
        public static TimeSpan CookieExpiration { get; } = TimeSpan.FromMinutes(60);
        public static string HeaderName { get; } = "X-CSRF-Token";
        public static string FormFieldName { get; } = "__SimulasiAPBN_Anti_Forgery_Token__";
    }
}