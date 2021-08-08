/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public enum JoinQueryType
    {
        InnerJoin,
        FullOuterJoin,
        LeftJoin,
        RightJoin
    }
}