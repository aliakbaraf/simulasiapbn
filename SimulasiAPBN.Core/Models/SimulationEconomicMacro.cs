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
    [DapperTable("SimulationEconomicMacros")]
    public sealed class SimulationEconomicMacro : GenericModel
    {
        private SimulationSession _simulationSession;
        private EconomicMacro _economicMacro;
        
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

        // M-1 relation with EconomicMacros
        [DapperWrite(false)]
        [Required]
        public EconomicMacro EconomicMacro
        {
            get => _economicMacro;
            set
            {
                _economicMacro = value;
                EconomicMacrosId = _economicMacro?.Id ?? Guid.Empty;
            }
        }
        [JsonIgnore]
        public Guid EconomicMacrosId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal UsedValue { get; set; }
        

    }
}