/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import EconomicMacro from "@core/models/EconomicMacro"
import StateBudget from "@core/models/StateBudget"

interface SimulationStateBudget extends GenericModel {
	state_budgets: StateBudget[]
	economic_macros: EconomicMacro[]
	used_income: number
}

export default SimulationStateBudget
