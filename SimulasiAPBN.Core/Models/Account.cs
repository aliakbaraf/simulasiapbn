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
    [DapperTable("Accounts")]
    public sealed class Account : GenericModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public AccountRole Role { get; set; }

        [Required]
        public bool IsActivated { get; set; }
        
        public override string ToString()
        {
            Password = null;
            return base.ToString();
        }
    }
}
