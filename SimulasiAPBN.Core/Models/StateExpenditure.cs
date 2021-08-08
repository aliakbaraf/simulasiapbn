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
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    [DapperTable("StateExpenditures")]
    public sealed class StateExpenditure : GenericModel
    {
        private StateBudget _stateBudget;
        private Budget _budget;

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
        public Guid StateBudgetId { get; set; }
        
        [DapperWrite(false)]
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
        public Guid BudgetId { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        [Required]
        public decimal TotalAllocation { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        [Required]
        public decimal SimulationMaximumMultiplier { get; set; }
        
        // M-1 relation with StateExpenditureAllocation
        [DapperWrite(false)]
        public ICollection<StateExpenditureAllocation> StateExpenditureAllocations { get; set; }
        
        // M-1 relation with SimulationStateExpenditure
        [DapperWrite(false)]
        public ICollection<SimulationStateExpenditure> SimulationStateExpenditures { get; set; }

    }
}
