/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using System.Linq;
using FluentValidation.Results;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Models;
using SimulasiAPBN.Web.Validation.Common;
using SimulasiAPBN.Web.Validation.Validators;

namespace SimulasiAPBN.Web.Validation
{
    public class ValidatorFactory : IValidatorFactory
    {
        public ValidatorFactory()
        {
            Allocation = new AllocationValidator();
            Budget = new BudgetValidator();
            ChangeAccount = new ChangeAccountValidator();
            ChangePassword = new ChangePasswordValidator();
            EconomicMacro = new EconomicMacroValidator();
            EditAnalyst = new EditAnalystValidator();
            RegisterAdministrator = new RegisterAdministratorValidator();
            Session = new SessionValidator();
            SignIn = new SignInValidator();
            SimulationSpecialPolicyAllocation = new SimulationSpecialPolicyAllocationValidator();
            SimulationStateExpenditure = new SimulationStateExpenditureValidator();
            SimulationEconomicMacro = new SimulationEconomicMacroValidator();
            SpecialPolicy = new SpecialPolicyValidator();
            StateBudget = new StateBudgetValidator();
            StateExpenditure = new StateExpenditureValidator();
            StateExpenditureAllocation = new StateExpenditureAllocationValidator();
            SpecialPolicyAllocation = new SpecialPolicyAllocationValidator();
            UpsertAnalyst = new UpsertAnalystValidator();
        }

        public Validator<Allocation> Allocation { get; }
        public Validator<Budget> Budget { get; }
        public Validator<Account> ChangeAccount { get; }
        public Validator<ChangePassword> ChangePassword { get; }
        public Validator<EconomicMacro> EconomicMacro { get; }
        public Validator<Account> EditAnalyst { get; }
        public Validator<Account> RegisterAdministrator { get; }
        public Validator<SimulationSession> Session { get; }
        public Validator<Account> SignIn { get; }
        public Validator<SimulationSpecialPolicyAllocation> SimulationSpecialPolicyAllocation { get; }
        public Validator<SimulationStateExpenditure> SimulationStateExpenditure { get; }
        public Validator<SimulationEconomicMacro> SimulationEconomicMacro { get; }
        public Validator<SpecialPolicy> SpecialPolicy { get; }
        public Validator<StateBudget> StateBudget { get; }
        public Validator<StateExpenditure> StateExpenditure { get; }
        public Validator<StateExpenditureAllocation> StateExpenditureAllocation { get; }
        public Validator<SpecialPolicyAllocation> SpecialPolicyAllocation { get; }
        public Validator<Account> UpsertAnalyst { get; }

        public string GetErrorMessage(ValidationResult validationResult)
        {
            return GetErrorMessage(validationResult, 
                "Data yang Anda masukan tidak valid.");
        }
        
        public string GetErrorMessage(ValidationResult validationResult, string defaultErrorMessage)
        {
            var errors = validationResult.Errors.ToList();
            return errors.Any()
                ? errors.First().ErrorMessage
                : defaultErrorMessage;
        }

        public void ThrowIfInvalid(ValidationResult validationResult)
        {
            if (validationResult.IsValid) return;
            var message = GetErrorMessage(validationResult);
            throw new BadRequestException(message);
        }
    }
}