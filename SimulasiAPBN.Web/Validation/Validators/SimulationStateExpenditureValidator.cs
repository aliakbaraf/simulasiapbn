using FluentValidation;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Validation.Common;

namespace SimulasiAPBN.Web.Validation.Validators
{
	public class SimulationStateExpenditureValidator : Validator<SimulationStateExpenditure>
	{
		public SimulationStateExpenditureValidator()
		{
			RuleFor(model => model.Id)
				.NotNull()
				.WithMessage("Identitas Belanja Negara diperlukan.");
			RuleFor(model => model.Id)
				.NotEmpty()
				.WithMessage("Identitas Belanja Negara diperlukan.");
			RuleFor(model => model.TotalAllocation)
				.NotNull()
				.WithMessage("Silakan masukan Total Alokasi.");
			RuleFor(model => model.TotalAllocation)
				.NotEmpty()
				.WithMessage("Silakan masukan Total Alokasi.");
			RuleFor(model => model.TotalAllocation)
				.GreaterThan(0)
				.WithMessage("Alokasi harus lebih dari Rp 0.");
		}
	}
}