/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import StateBudget from "@core/models/StateBudget"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import SimulationShare from "@core/models/SimulationShare"
import SimulationState from "@core/enums/SimulationState"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"

interface SimulationSession extends GenericModel {
	name: string
	used_income: number
	simulation_state: SimulationState
	state_budget: StateBudget
	simulation_state_expenditures: SimulationStateExpenditure[]
	simulation_shares: SimulationShare[]
	simulation_special_policy_allocations: SimulationSpecialPolicyAllocation[]
	simulation_economic_macros: SimulationEconomicMacro[]
}

export default SimulationSession
