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
    // For Policy
    [DapperTable("SpecialPolicyAllocations")]
    public sealed class SpecialPolicyAllocation : GenericModel
    {
        private SpecialPolicy _specialPolicy;
        private Allocation _allocation;
        
        // M-1 relation with SpecialPolicy
        [DapperWrite(false)]
        [JsonIgnore]
        [Required]
        public SpecialPolicy SpecialPolicy
        {
            get => _specialPolicy;
            set
            {
                _specialPolicy = value;
                SpecialPolicyId = _specialPolicy?.Id ?? Guid.Empty;
            }
        }
        public Guid SpecialPolicyId { get; set; }
        
        // M-1 relation with Allocation
        [DapperWrite(false)]
        [Required]
        public Allocation Allocation
        {
            get => _allocation;
            set
            {
                _allocation = value;
                AllocationId = _allocation?.Id ?? Guid.Empty;
            }
        }
        public Guid AllocationId { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        [Required]
        public decimal TotalAllocation { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        [Required]
        public decimal Percentage { get; set; }
        
        [Column(TypeName = "decimal(6,2)")]
        [Required]
        public decimal SimulationMaximumMultiplier { get; set; }
        
        // M-1 relation with SimulationSpecialPolicyAllocations
        [DapperWrite(false)]
        public ICollection<SimulationSpecialPolicyAllocation> SimulationSpecialPolicyAllocations { get; set; }

    }
}