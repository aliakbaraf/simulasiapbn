/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import SpecialPolicy from "@core/models/SpecialPolicy"
import StateExpenditure from "@core/models/StateExpenditure"

interface StateBudget extends GenericModel {
	year: number
	revision: number
	country_income: number
	special_policies: SpecialPolicy[]
	state_expenditures: StateExpenditure[]
}

export default StateBudget
