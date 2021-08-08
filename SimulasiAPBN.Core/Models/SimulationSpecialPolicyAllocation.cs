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
    [DapperTable("SimulationSpecialPolicyAllocations")]
    public sealed class SimulationSpecialPolicyAllocation : GenericModel
    {
        private SimulationSession _simulationSession;
        private SpecialPolicyAllocation _specialPolicyAllocation;
        
        // M-1 relation with SimulationSession

        [DapperWrite(false)]
        [JsonIgnore]
        [Required]
        public SimulationSession SimulationSession
        {
            get => _simulationSession;
            set
            {
                _simulationSession = value;
                SimulationSessionId = _simulationSession?.Id ?? Guid.Empty;
            }
        }
        
        [JsonIgnore]
        public Guid SimulationSessionId { get; set; }
        
        // M-1 relation with SpecialPolicyAllocation
        [DapperWrite(false)]
        [Required]
        public SpecialPolicyAllocation SpecialPolicyAllocation
        {
            get => _specialPolicyAllocation;
            set
            {
                _specialPolicyAllocation = value;
                SpecialPolicyAllocationId = _specialPolicyAllocation?.Id ?? Guid.Empty;
            }
        }
        [JsonIgnore]
        public Guid SpecialPolicyAllocationId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalAllocation { get; set; }
        
    }
}