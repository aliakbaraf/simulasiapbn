/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SimulasiAPBN.Core.Enumerators;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    [DapperTable("SignInAttempts")]
    public sealed class SignInAttempt : GenericModel
    {
        private Account _account;
        
        // M-1 relation with Account

        [DapperWrite(false)]
        [JsonIgnore]
        [Required]
        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                AccountId = _account?.Id ?? Guid.Empty;
            }
        }
        public Guid AccountId { get; set; }
        
        [Required]
        public string UserAgent { get; set; }
        
        [Required]
        public string IpAddress { get; set; }
        
        [Required]
        public bool IsSuccess { get; set; }
        
        public SignInStatusCode StatusCode { get; set; }

    }
}