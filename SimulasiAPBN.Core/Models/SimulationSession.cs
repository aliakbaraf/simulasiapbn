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
using SimulasiAPBN.Core.Enumerators;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    // For Simulation
    [DapperTable("SimulationSessions")]
    public sealed class SimulationSession : GenericModel
    {
        private StateBudget _stateBudget;

        [Required]
        public string Name { get; set; }
        
        [Required]
        [JsonIgnore]
        public string EngineKey { get; set; }
        
        [Required]
        public SimulationState SimulationState { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal UsedIncome { get; set; }

        // 1-M relation with SimulationState
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

        // 1-M relation with SimulationBudgetData
        [DapperWrite(false)]
        public ICollection<SimulationStateExpenditure> SimulationStateExpenditures { get; set; }

        // 1-M relation with SimulationEconomicMacro
        [DapperWrite(false)]
        public ICollection<SimulationEconomicMacro> SimulationEconomicMacros { get; set; }

        // 1-M relation with SimulationShare
        [DapperWrite(false)]
        public ICollection<SimulationShare> SimulationShares { get; set; }
        
        // 1-M relation with SimulationSpecialPolicyAllocation
        [DapperWrite(false)]
        public ICollection<SimulationSpecialPolicyAllocation> SimulationSpecialPolicyAllocations
        {
            get; 
            set;
        }

    }
}
