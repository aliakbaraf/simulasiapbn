/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;

namespace SimulasiAPBN.Common.Configuration
{
    public static class AuthenticationAuthorization
    {
        public static string AccessDeniedPath { get; } = "/error/accessdenied";
        public static string DefaultScheme { get; } = "SimulasiAPBN.AAA";
        public static bool CookieHttpOnly { get; } = true;
        public static string CookieName { get; } = DefaultScheme + ".Client";
        public static TimeSpan ExpireTimeSpan { get; } = TimeSpan.FromDays(3);
        public static string ReturnUrlQuery { get; } = "ReturnUrl";
        public static string SignInPath { get; } = "/gate/signin";
        public static string SignOutPath { get; } = "/gate/signout";
    }

}