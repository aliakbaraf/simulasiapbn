/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public static class JoinQueryTypeExtension
    {
        public static string GetKeyword(this JoinQueryType type)
        {
            return type switch
            {
                JoinQueryType.InnerJoin => "INNER JOIN",
                JoinQueryType.FullOuterJoin => "FULL OUTER JOIN",
                JoinQueryType.LeftJoin => "LEFT JOIN",
                JoinQueryType.RightJoin => "RIGHT JOIN",
                _ => "JOIN"
            };
        }
    }
}