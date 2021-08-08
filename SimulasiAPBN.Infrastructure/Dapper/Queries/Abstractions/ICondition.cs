/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions
{
	public interface ICondition<TValue>
	{
		public int Count { get; }
		
		TValue this[string field] { get; set; }

		IList<string> Fields { get; }
		IList<string> Tables { get; }
			
		IList<string> Operators { get; }

		IList<TValue> Values { get; }

		bool Contains(string table, string field);

		void Add(string table, string field, string @operator, TValue value);

		bool Any();

		bool Remove(string table, string field);

		IEnumerable<string> ToConditionString();

		bool TryGetValue(
			string table,
			string field, 
			[MaybeNullWhen(false)] out string @operator, 
			[MaybeNullWhen(false)] out TValue value);
	}
}