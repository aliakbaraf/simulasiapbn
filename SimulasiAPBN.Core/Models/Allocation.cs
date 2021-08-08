/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;

namespace SimulasiAPBN.Core.Models
{
    // For Simulation & Policy
    [DapperTable("Allocations")]
    public sealed class Allocation : GenericModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public bool IsMandatory { get; set; }
        
        public string MandatoryExplanation { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal MandatoryThreshold { get; set; }
    }
}
