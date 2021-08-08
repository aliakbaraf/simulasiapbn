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
    // For Simulation
    [DapperTable("SimulationStateExpenditures")]
    public sealed class SimulationStateExpenditure : GenericModel
    {
        private SimulationSession _simulationSession;
        private StateExpenditure _stateExpenditure;
        
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

        // M-1 relation with StateExpenditures
        [DapperWrite(false)]
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
        
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalAllocation { get; set; }
        
        [Required]
        public bool IsPriority { get; set; }

    }
}