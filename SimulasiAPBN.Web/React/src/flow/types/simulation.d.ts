/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import SimulationSession from "@core/models/SimulationSession"
import Allocation from "@core/models/Allocation"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import Guid from "@core/types/Guid"
import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import StateBudget from "@core/models/StateBudget"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"

export type SimulationExpenditure = {
	all: SimulationStateExpenditure[]
	centralGovernment: SimulationStateExpenditure[]
	transferToRegional: SimulationStateExpenditure[]
}

export type SimulationProgress = {
	economicMacroDone: boolean
	centralGovernmentDone: boolean
	transferToRegionalDone: boolean
	specialPoliciesDone: {
		[key: string]: boolean
	}
}

export type SimulationSpecialPolicy = {
	id: Guid
	name: string
	description: string
	totalAllocation: number
	specialPolicyAllocations: SimulationSpecialPolicyAllocation[]
}

export type SimulationTotalExpenditure = {
	all: number
	centralGovernment: number
	transferToRegional: number
}

export type SimulationEconomic = {
	all: SimulationEconomicMacro[]
	economicMacro: SimulationEconomicMacro[]
}

export type SimulationTotalEconomic = {
	all: number
	economicMacro: number
}

export type SimulationState = {
	allocations: Allocation[]
	expenditures: SimulationExpenditure
	progress: SimulationProgress
	session?: SimulationSession
	specialPolicies: SimulationSpecialPolicy[]
	economicMacros: SimulationEconomic
	stateBudget: StateBudget
	totalExpenditure: SimulationTotalExpenditure
	totalEconomicMacro: SimulationTotalEconomic
}

export default SimulationState
