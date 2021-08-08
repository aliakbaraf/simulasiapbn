/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SimulasiAPBN.Infrastructure.Dapper.Queries.Abstractions;

namespace SimulasiAPBN.Infrastructure.Dapper.Queries
{
	public class Condition<TValue> : ICondition<TValue>
	{
		public Condition()
		{
			Fields = new Collection<string>();
			Tables = new Collection<string>();
			Operators = new Collection<string>();
			Values = new Collection<TValue>();
		}
		
		public int Count => Fields.Count;

		public TValue this [string field]
		{
			get
			{
				var index = Fields.IndexOf(field);
				if (index == -1)
				{
					throw new ArgumentOutOfRangeException(field);
				}

				return Values[index];
			}
			set
			{
				var index = Fields.IndexOf(field);
				if (index == -1)
				{
					Fields.Add(field);
					Values.Add(value);
				}
				else
				{
					Values[index] = value;
				}
			}
		}

		public IList<string> Fields { get; }
		public IList<string> Tables { get; }
		public IList<string> Operators { get; }
		public IList<TValue> Values { get; }
		
		public bool Contains(string table, string field)
		{
			var index = Fields.IndexOf(field);
			if (index == -1)
			{
				return false;
			}

			return Tables[index] == table;
		}

		public void Add(string table, string field, string @operator, TValue value)
		{
			Tables.Add(table);
			Fields.Add(field);
			Operators.Add(@operator);
			Values.Add(value);
		}

		public bool Any()
		{
			return Fields.Any();
		}

		public bool Remove(string table, string field)
		{
			var index = Fields.IndexOf(field);
			if (index == -1)
			{
				return false;
			}

			if (Tables[index] != table)
			{
				return false;
			}
			
			Tables.RemoveAt(index);
			Fields.RemoveAt(index);
			Operators.RemoveAt(index);
			Values.RemoveAt(index);
			return true;
		}

		public IEnumerable<string> ToConditionString()
		{
			var result = new Collection<string>();
			
			var fields = Fields.Select((field, index)  => (field, index));
			foreach (var (field, index) in fields)
			{
				var table = Tables[index];
				var @operator = Operators[index];
				var value = Values[index];
				result.Add($"[{table}].[{field}] {@operator} {value}");
			}

			return result;
		}

		public bool TryGetValue(
			string table,
			string field, 
			[MaybeNullWhen(false)] out string @operator, 
			[MaybeNullWhen(false)] out TValue value)
		{
			var index = Fields.IndexOf(field);
			if (index == -1)
			{
				@operator = null;
				value = default;
				return false;
			}

			@operator = Operators[index];
			value = Values[index];
			return true;
		}
		
	}
}