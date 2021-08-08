/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    [DapperTable("EconomicMacros")]
    public sealed class EconomicMacro : GenericModel
    {
        private StateBudget _stateBudget;

        // M-1 relation with StateBudget
        [DapperWrite(false)]
        [Required]
        public StateBudget StateBudget
        {
            get => _stateBudget;
            set
            {
                _stateBudget = value;
                StateBudgetId = _stateBudget?.Id ?? Guid.Empty;
            }
        }
        [JsonIgnore]
        public Guid StateBudgetId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Naration { get; set; }

        [Required]
        public string NarationDefisit { get; set; }

        [Required]
        public string UnitDesc { get; set; }

        [Required]
        public int OrderFlag { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal DefaultValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal MinimumValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal MaximumValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Threshold { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ThresholdValue { get; set; }

        // M-1 relation with SimulationEconomiMacro
        [DapperWrite(false)]
        public ICollection<SimulationEconomicMacro> SimulationEconomicMacros { get; set; }

    }
}
