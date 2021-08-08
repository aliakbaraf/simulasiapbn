/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace SimulasiAPBN.Web.Validation.Common
{
    public class Validator<T> : AbstractValidator<T> 
        where T : class
    {
        
        public Validation GetValidation(T model)
        {
            var validationResult = Validate(model);
            return new Validation(validationResult.Errors);
        }
        
        public async Task<Validation> GetValidationAsync(
            T model, 
            CancellationToken cancellation = new CancellationToken())
        {
            if (model is null)
            {
                return new Validation();
            }
            var validationResult = await ValidateAsync(model, cancellation);
            return new Validation(validationResult.Errors);
        }
    }
}