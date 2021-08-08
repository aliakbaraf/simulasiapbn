/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Models.Engine;

namespace SimulasiAPBN.Web.Validation.Common
{
    public class Validation : ValidationResult
    {

        public Validation() : this(new List<ValidationFailure>())
        {
        }
        
        public Validation(IEnumerable<ValidationFailure> failures) : base(failures)
        {
        }
        
        public string GetFirstErrorMessage()
        {
            return GetFirstOrDefaultErrorMessage("Data yang Anda masukan tidak valid.");
        }
        
        public string GetFirstOrDefaultErrorMessage(string defaultErrorMessage)
        {
            var errors = Errors.ToList();
            return errors.Any()
                ? errors.First().ErrorMessage
                : defaultErrorMessage;
        }
        
        public void ThrowIfInvalid()
        {
            if (IsValid) {
                return;
            }

            throw new BadRequestException(GetFirstErrorMessage(), EngineErrorCode.DataValidationFailed);
        }
    }
}