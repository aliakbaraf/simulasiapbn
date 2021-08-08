/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import Allocation from "@core/models/Allocation"
import Guid from "@core/types/Guid"

interface SpecialPolicyAllocation extends GenericModel {
	special_policy_id: Guid
	allocation: Allocation
	allocation_id: Guid
	total_allocation: number
	percentage: number
	simulation_maximum_multiplier: number
}

export default SpecialPolicyAllocation
