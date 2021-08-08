/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    // For Simulation & Policy
    [DapperTable("StateExpenditureAllocations")]
    public sealed class StateExpenditureAllocation : GenericModel
    {
        private StateExpenditure _stateExpenditure;
        private Allocation _allocation;
        
        // M-1 relation with StateExpenditure
        [DapperWrite(false)]
        [JsonIgnore]
        [Required]
        public StateExpenditure StateExpenditure
        {
            get => _stateExpenditure;
            set
            {
                _stateExpenditure = value;
                StateExpenditureId = _stateExpenditure?.Id ?? Guid.Empty;
            }
        }
        [JsonIgnore]
        public Guid StateExpenditureId { get; set; }

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

    }
}
