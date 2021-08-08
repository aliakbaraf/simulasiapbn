/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SimulasiAPBN.Common.Serializer;
using DapperKey = Dapper.Contrib.Extensions.KeyAttribute;

namespace SimulasiAPBN.Core.Models
{
    public abstract class GenericModel
    {
        protected GenericModel()
        {
            Id = Guid.Empty;
        }

        [DapperKey]
        [Key]
        [Required]
        public Guid Id { get; set; }

        [JsonIgnore]
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonIgnore]
        public DateTimeOffset? UpdatedAt { get; set; }
        
        [JsonIgnore]
        public DateTimeOffset? DeletedAt { get; set; }

        public override string ToString()
        {
            return Json.Serialize(this);
        }
    }
}
