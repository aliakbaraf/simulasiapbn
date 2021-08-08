/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using FluentValidation.Results;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation
{
    public interface IValidatorFactory
    {
        Validator<Allocation> Allocation { get; }
        Validator<Budget> Budget { get; }
        Validator<Account> ChangeAccount { get; }
        Validator<ChangePassword> ChangePassword { get; }
        Validator<EconomicMacro> EconomicMacro { get; }
        Validator<Account> EditAnalyst { get; }
        Validator<Account> RegisterAdministrator { get; }
        Validator<SimulationSession> Session { get; }
        Validator<Account> SignIn { get; }
        Validator<SimulationSpecialPolicyAllocation> SimulationSpecialPolicyAllocation { get; }
        Validator<SimulationStateExpenditure> SimulationStateExpenditure { get; }
        Validator<SimulationEconomicMacro> SimulationEconomicMacro { get; }
        Validator<SpecialPolicy> SpecialPolicy { get; }
        Validator<StateBudget> StateBudget { get; }
        Validator<StateExpenditure> StateExpenditure { get; }
        Validator<StateExpenditureAllocation> StateExpenditureAllocation { get; }
        Validator<SpecialPolicyAllocation> SpecialPolicyAllocation { get; }
        Validator<Account> UpsertAnalyst { get; }

        string GetErrorMessage(ValidationResult validationResult);
        string GetErrorMessage(ValidationResult validationResult, string defaultErrorMessage);
        void ThrowIfInvalid(ValidationResult validationResult);
    }
}