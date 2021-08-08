/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Text;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
    public class Query
    {
        protected readonly StringBuilder QueryStringBuilder;
        
        public Query(string query)
        {
            QueryStringBuilder = new StringBuilder();
            QueryStringBuilder.Append(query);
        }
        
        public Query(StringBuilder queryStringBuilder)
        {
            QueryStringBuilder = queryStringBuilder;
        }

        public override string ToString()
        {
            return QueryStringBuilder.ToString();
        }
    }
}