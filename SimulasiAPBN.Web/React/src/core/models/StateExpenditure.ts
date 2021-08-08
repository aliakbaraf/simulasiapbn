/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import Budget from "@core/models/Budget"
import StateBudget from "@core/models/StateBudget"
import StateExpenditureAllocation from "@core/models/StateExpenditureAllocation"
import Guid from "@core/types/Guid"

interface StateExpenditure extends GenericModel {
	state_budget_id: Guid
	state_budget: StateBudget
	budget_id: Guid
	budget: Budget
	total_allocation: number
	simulation_maximum_multiplier: number
	state_expenditure_allocations: StateExpenditureAllocation[]
}

export default StateExpenditure
