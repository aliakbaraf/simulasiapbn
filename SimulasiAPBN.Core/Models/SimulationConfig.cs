/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.ComponentModel.DataAnnotations;
using SimulasiAPBN.Core.Enumerators;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;

namespace SimulasiAPBN.Core.Models
{
    [DapperTable("SimulationConfigs")]
    public sealed class SimulationConfig : GenericModel
    {
        
        [Required]
        public SimulationConfigKey Key { get; set; }

        [Required]
        public string Value { get; set; }
        
    }
}