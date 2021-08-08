/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import SpecialPolicyAllocation from "@core/models/SpecialPolicyAllocation"

interface SimulationSpecialPolicyAllocation extends GenericModel {
	special_policy_allocation: SpecialPolicyAllocation
	total_allocation: number
}

export default SimulationSpecialPolicyAllocation
