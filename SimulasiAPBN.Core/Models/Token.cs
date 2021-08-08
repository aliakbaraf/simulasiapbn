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
    [DapperTable("Tokens")]
    public sealed class Token : GenericModel
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
        public string Code { get; set; }

        [Required]
        public TokenType Type { get; set; }

    }
}
