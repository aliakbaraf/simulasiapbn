/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SimulasiAPBN.Core.Enumerators;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    // For Simulation
    [DapperTable("SimulationShares")]
    public class SimulationShare : GenericModel
    {
        private SimulationSession _simulationSession;
        
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
        
        [Required]
        public SimulationShareTarget Target { get; set; }
        
        [Required]
        public int ClickedTimes { get; set; }
    }
}