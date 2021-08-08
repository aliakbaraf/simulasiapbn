/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Web.Models
{
	public class DataModel<T> where T : GenericModel
	{

		public IEnumerable<T> Data { get; set; }
		
	}
}