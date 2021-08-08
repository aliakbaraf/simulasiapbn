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
    // For Policy
    [DapperTable("BudgetTargets")]
    public sealed class BudgetTarget : GenericModel
    {
        private Budget _budget;

        [Required]
        public string Description { get; set; }

        // M-1 relation with StateExpenditure
        [DapperWrite(false)]
        [JsonIgnore]
        [Required]
        public Budget Budget
        {
            get => _budget;
            set
            {
                _budget = value;
                BudgetId = _budget?.Id ?? Guid.Empty;
            }
        }
        [JsonIgnore]
        public Guid BudgetId { get; set; }
    }
}