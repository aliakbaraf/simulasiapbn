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
    [DapperTable("MediaFiles")]
    public sealed class MediaFile : GenericModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string File { get; set; }
        
        [Required]
        public MediaType Type { get; set; }
    }
}