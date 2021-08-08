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
    [DapperTable("WebContents")]
    public sealed class WebContent : GenericModel
    {

        [Required]
        public WebContentKey Key { get; set; }

        [Required]
        public string Value { get; set; }

    }
}
