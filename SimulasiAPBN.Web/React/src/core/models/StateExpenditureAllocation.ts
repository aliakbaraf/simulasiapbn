/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import Allocation from "@core/models/Allocation"
import Guid from "@core/types/Guid"

interface StateExpenditureAllocation extends GenericModel {
	allocation_id: Guid
	allocation: Allocation
	total_allocation: number
	percentage: number
}

export default StateExpenditureAllocation
