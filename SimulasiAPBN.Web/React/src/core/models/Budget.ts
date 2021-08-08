/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import BudgetTarget from "@core/models/BudgetTarget"
import BudgetType from "@core/enums/BudgetType"

interface Budget extends GenericModel {
	function: string
	type: BudgetType
	description: string
	budget_targets: BudgetTarget[]
}

export default Budget
