import StateBudget from "@core/models/StateBudget"

class Formatter {
	public getStateBudgetName(stateBudget?: StateBudget): string {
		if (typeof stateBudget === "undefined" || stateBudget === null) {
			return "APBN"
		}

		return stateBudget.revision === 0
			? `APBN Tahun ${stateBudget.year}`
			: `APBN Tahun ${stateBudget.year} Perubahan ke-${stateBudget.revision}`
	}
}

const useFormatter = () => new Formatter()

export default useFormatter
