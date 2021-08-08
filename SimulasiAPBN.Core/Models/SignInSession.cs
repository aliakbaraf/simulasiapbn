/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
	[DapperTable("SignInSessions")]
	public sealed class SignInSession : GenericModel
	{
		private SignInAttempt _signInAttempt;
        
		// M-1 relation with Account

		[DapperWrite(false)]
		[JsonIgnore]
		[Required]
		public SignInAttempt SignInAttempt
		{
			get => _signInAttempt;
			set
			{
				_signInAttempt = value;
				SignInAttemptId = _signInAttempt?.Id ?? Guid.Empty;
			}
		}
		public Guid SignInAttemptId { get; set; }
        
		public bool IsRevoked { get; set; }

	}
}