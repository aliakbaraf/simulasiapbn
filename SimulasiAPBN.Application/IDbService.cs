/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Data.Common;

namespace SimulasiAPBN.Application
{
    public interface IDbService
    {
        string ConnectionString { get; }
        
        DbConnection SqlConnection { get; }
    }
}