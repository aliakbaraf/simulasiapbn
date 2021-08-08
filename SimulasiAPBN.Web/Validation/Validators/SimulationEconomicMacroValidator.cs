using FluentValidation;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation.Validators
{
	public class SimulationEconomicMacroValidator : Validator<SimulationEconomicMacro>
	{
		public SimulationEconomicMacroValidator()
		{
			RuleFor(model => model.Id)
				.NotNull()
				.WithMessage("Identitas Ekonomi Makro diperlukan.");
			RuleFor(model => model.Id)
				.NotEmpty()
				.WithMessage("Identitas Ekonomi Makro diperlukan.");			
		}
	}
}