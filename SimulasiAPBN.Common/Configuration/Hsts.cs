/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;

namespace SimulasiAPBN.Common.Configuration
{
    public static class Hsts
    {
        public static bool Preload { get; } = true;
        public static bool IncludeSubDomains { get; } = true;
        public static TimeSpan MaxAge { get; } = TimeSpan.FromHours(1);
    }
}