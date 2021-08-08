/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import SimulationShareTarget from "@core/enums/SimulationShareTarget"

interface SimulationShare extends GenericModel {
	target: SimulationShareTarget
	clicked_times: number
}

export default SimulationShare
