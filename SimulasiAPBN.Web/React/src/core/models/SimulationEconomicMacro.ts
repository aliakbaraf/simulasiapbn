/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import EconomicMacro from "@core/models/EconomicMacro"

interface SimulationEconomicMacro extends GenericModel {
	economic_macro: EconomicMacro
	used_value: number
}

export default SimulationEconomicMacro
