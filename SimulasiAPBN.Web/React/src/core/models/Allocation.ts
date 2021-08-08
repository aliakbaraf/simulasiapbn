/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"

interface Allocation extends GenericModel {
	name: string
	is_mandatory: boolean
	mandatory_explanation?: string
	mandatory_threshold?: number
}

export default Allocation
