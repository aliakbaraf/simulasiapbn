/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import StateExpenditure from "@core/models/StateExpenditure"

interface SimulationStateExpenditure extends GenericModel {
	state_expenditure: StateExpenditure
	total_allocation: number
	is_priority: boolean
}

export default SimulationStateExpenditure
