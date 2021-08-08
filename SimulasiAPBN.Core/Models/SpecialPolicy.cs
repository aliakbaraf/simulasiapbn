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
    // For Simulation & Policy
    [DapperTable("SpecialPolicies")]
    public sealed class SpecialPolicy : GenericModel
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
        public bool IsActive { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalAllocation { get; set; }
        
        // 1-M relation with SpecialPolicyAllocation
        [DapperWrite(false)]
        public ICollection<SpecialPolicyAllocation> SpecialPolicyAllocations { get; set; }
        
        
    }
}