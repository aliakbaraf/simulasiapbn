/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DapperTable = Dapper.Contrib.Extensions.TableAttribute;
using DapperWrite = Dapper.Contrib.Extensions.WriteAttribute;

namespace SimulasiAPBN.Core.Models
{
    [DapperTable("StateBudgets")]
    public sealed class StateBudget : GenericModel
    {
        
        [Required]
        public int Year { get; set; } = DateTimeOffset.Now.Year;

        [Required]
        public int Revision { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,6)")]
        [Required]
        public decimal CountryIncome { get; set; }
        
        // M-1 relation with StateExpenditure
        [DapperWrite(false)]
        public ICollection<StateExpenditure> StateExpenditures { get; set; }
        
        // M-1 relation with SpecialPolicy
        [DapperWrite(false)]
        public ICollection<SpecialPolicy> SpecialPolicies { get; set; }

        // M-1 relation with EconomicMacro
        [DapperWrite(false)]
        public ICollection<EconomicMacro> EconomicMacros { get; set; }

    }
}
