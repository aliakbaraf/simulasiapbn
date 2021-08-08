/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import StateBudget from "@core/models/StateBudget"
import SpecialPolicyAllocation from "@core/models/SpecialPolicyAllocation"

interface SpecialPolicy extends GenericModel {
	state_budget: StateBudget
	name: string
	description: string
	is_active: boolean
	total_allocation: number
	special_policy_allocations: SpecialPolicyAllocation[]
}

export default SpecialPolicy
