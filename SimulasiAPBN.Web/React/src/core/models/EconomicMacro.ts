/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"

interface EconomicMacro extends GenericModel {
	name: string
	unit_desc: string
	order_flag: number
	description: string
	naration: string
	naration_defisit: string
	default_value: number
	minimum_value: number
	maximum_value: number
	threshold: number
	threshold_value: number
	economic_macros: EconomicMacro[]
}

export default EconomicMacro
