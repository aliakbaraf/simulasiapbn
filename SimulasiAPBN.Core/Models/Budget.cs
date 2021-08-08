/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimulasiAPBN.Core.Enumerators;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    // For Simulation & Policy
    [DapperTable("Budgets")]
    public sealed class Budget : GenericModel
    {

        [Required]
        public string Function { get; set; }

        [Required]
        public BudgetType Type { get; set; }
        
        [Required]
        public string Description { get; set; }

        [DapperWrite(false)]
        public ICollection<BudgetTarget> BudgetTargets { get; set; }

    }
}
