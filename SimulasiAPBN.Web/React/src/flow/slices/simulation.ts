/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { createSlice, PayloadAction, SliceCaseReducers } from "@reduxjs/toolkit"

import ClientSimulation from "@core/types/ClientSimulation"
import SimulationState, { SimulationSpecialPolicy } from "@flow/types/simulation"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import BudgetType from "@core/enums/BudgetType"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"

const sortString = (a: string, b: string) => {
	return a < b ? -1 : a > b ? 1 : 0
}

const sortNumber = (a: number, b: number) => {
	return a < b ? -1 : a > b ? 1 : 0
}

const slice = createSlice<SimulationState, SliceCaseReducers<SimulationState>>({
	name: "simulation",
	initialState: {
		allocations: [],
		expenditures: {
			all: [],
			centralGovernment: [],
			transferToRegional: [],
		},
		progress: {
			economicMacroDone: false,
			centralGovernmentDone: false,
			transferToRegionalDone: false,
			specialPoliciesDone: {},
		},
		specialPolicies: [],
		stateBudget: {
			id: "",
			year: new Date().getFullYear(),
			revision: 0,
			country_income: 0,
			state_expenditures: [],
			special_policies: [],
		},
		totalExpenditure: {
			all: 0,
			centralGovernment: 0,
			transferToRegional: 0,
		},
		economicMacros: {
			all: [],
			economicMacro: [],
		},
		totalEconomicMacro: {
			all: 0,
			economicMacro: 0,
		},
	},
	reducers: {
		setCentralGovernmentClientProgress: (state, action: PayloadAction<boolean>) => {
			state.progress = { ...state.progress, centralGovernmentDone: action.payload }
		},
		setTransferToRegionalClientProgress: (state, action: PayloadAction<boolean>) => {
			state.progress = { ...state.progress, transferToRegionalDone: action.payload }
		},
		setEconomicMacroClientProgress: (state, action: PayloadAction<boolean>) => {
			state.progress = { ...state.progress, economicMacroDone: action.payload }
		},
		setSpecialPolicyClientProgress: (state, action: PayloadAction<[string, boolean]>) => {
			const [specialPolicyId, done] = action.payload
			const specialPoliciesDone = state.progress.specialPoliciesDone
			specialPoliciesDone[specialPolicyId] = done
			state.progress = { ...state.progress, specialPoliciesDone }
		},
		setClientSimulation: (state, action: PayloadAction<ClientSimulation | undefined>) => {
			// Clear session data
			if (typeof action.payload === "undefined") {
				state.allocations = []
				state.expenditures = {
					all: [],
					centralGovernment: [],
					transferToRegional: [],
				}
				state.progress = {
					economicMacroDone: false,
					centralGovernmentDone: false,
					transferToRegionalDone: false,
					specialPoliciesDone: {},
				}
				state.session = void 0
				state.specialPolicies = []
				state.stateBudget = {
					id: "",
					year: new Date().getFullYear(),
					revision: 0,
					country_income: 0,
					state_expenditures: [],
					special_policies: [],
				}
				state.totalExpenditure = {
					all: 0,
					centralGovernment: 0,
					transferToRegional: 0,
				}
				state.economicMacros = {
					all: [],
					economicMacro: [], 
				}
				state.totalEconomicMacro = {
					all: 0,
					economicMacro: 0,
                }
				return
			}

			const { allocations, session } = action.payload

			// Detects new session and reset progress
			const progress = { ...state.progress }

			if (typeof state.session === "undefined") {
				progress.economicMacroDone = false
				progress.centralGovernmentDone = false
				progress.transferToRegionalDone = false
				progress.specialPoliciesDone = {}

				const specialPolicies = (session.state_budget.special_policies || []).filter(
					specialPolicy => specialPolicy.is_active
				)
				for (const specialPolicy of specialPolicies) {
					progress.specialPoliciesDone[specialPolicy.id] = false
				}
			}

			// Process session: Linking allocation
			for (const [expenditureIndex, expenditure] of session.simulation_state_expenditures.entries()) {
				for (const [
					expenditureAllocationIndex,
					expenditureAllocation,
				] of expenditure.state_expenditure.state_expenditure_allocations.entries()) {
					const allocation = allocations.find(entity => entity.id === expenditureAllocation.allocation_id)
					if (typeof allocation === "undefined") {
						continue
					}

					session.simulation_state_expenditures[
						expenditureIndex
					].state_expenditure.state_expenditure_allocations[
						expenditureAllocationIndex
					].allocation = allocation
				}
			}

			// Process session: Linking allocation
			for (const [
				policyAllocationIndex,
				policyAllocation,
			] of session.simulation_special_policy_allocations.entries()) {
				const allocation = allocations.find(
					entity => entity.id === policyAllocation.special_policy_allocation.allocation_id
				)
				if (typeof allocation === "undefined") {
					continue
				}

				session.simulation_special_policy_allocations[
					policyAllocationIndex
				].special_policy_allocation.allocation = allocation
			}

			
			// Dividing data
			const allExpenditures: SimulationStateExpenditure[] = []
			const centralGovernmentExpenditures: SimulationStateExpenditure[] = []
			const transferToRegionalExpenditures: SimulationStateExpenditure[] = []
			const specialPolicies: SimulationSpecialPolicy[] = []

			const allEconomicMacros: SimulationEconomicMacro[] = []
			const economicMacros: SimulationEconomicMacro[] = []
			let totalAllExpenditures = 0
			let totalCentralGovernmentExpenditures = 0
			let totalTransferToRegionalExpenditures = 0
			let totalAllEconomicMacro = 0
			let totalEconomicMacroExpenditures = 0
			
			const sortExpenditures = session.simulation_state_expenditures.sort((a, b) =>
				sortString(a.state_expenditure.budget.function, b.state_expenditure.budget.function)
			)
			for (const expenditure of sortExpenditures) {
				allExpenditures.push(expenditure)
				totalAllExpenditures += expenditure.total_allocation
				switch (expenditure.state_expenditure.budget.type) {
					case BudgetType.CentralGovernmentExpenditure:
						centralGovernmentExpenditures.push(expenditure)
						totalCentralGovernmentExpenditures += expenditure.total_allocation
						break
					case BudgetType.TransferToRegionalExpenditure:
						transferToRegionalExpenditures.push(expenditure)
						totalTransferToRegionalExpenditures += expenditure.total_allocation
						break
					default:
						break
				}
			}

			//const sortEconomicMacros = session.simulation_economic_macros.sort((a, b) =>
			//	sortString(a.economic_macro.name, b.economic_macro.name)
			//)

			const sortEconomicMacros = session.simulation_economic_macros.sort((a, b) =>
				sortNumber(a.economic_macro.order_flag, b.economic_macro.order_flag)
			)

			for (const economic of sortEconomicMacros) {
				allEconomicMacros.push(economic)
				economicMacros.push(economic)
				totalAllEconomicMacro = typeof session.used_income !== "undefined" ? session.used_income : session.state_budget.country_income
				totalEconomicMacroExpenditures = typeof session.used_income !== "undefined" ? session.used_income : session.state_budget.country_income
			}

			if (centralGovernmentExpenditures.length <= 0) {
				progress.centralGovernmentDone = true
			}
			if (transferToRegionalExpenditures.length <= 0) {
				progress.transferToRegionalDone = true
			}
			if (economicMacros.length <= 0) {
				progress.economicMacroDone = true
            }

			const sortedSimulationSpecialPolicyAllocations = session.simulation_special_policy_allocations.sort(
				(a, b) =>
					sortString(a.special_policy_allocation.allocation.name, b.special_policy_allocation.allocation.name)
			)
			for (const specialPolicy of session.state_budget.special_policies.sort((a, b) =>
				sortString(a.name, b.name)
			)) {
				if (!specialPolicy.is_active) {
					continue
				}

				const specialPolicyAllocations = sortedSimulationSpecialPolicyAllocations.filter(
					entity => entity.special_policy_allocation.special_policy_id === specialPolicy.id
				)
				if (specialPolicyAllocations.length <= 0) {
					continue
				}

				const simulationSpecialPolicy: SimulationSpecialPolicy = {
					id: specialPolicy.id,
					name: specialPolicy.name,
					description: specialPolicy.description,
					totalAllocation: specialPolicy.total_allocation,
					specialPolicyAllocations:
						typeof specialPolicyAllocations !== "undefined" ? specialPolicyAllocations : [],
				}

				specialPolicies.push(simulationSpecialPolicy)
			}

			state.allocations = allocations
			state.expenditures = {
				all: allExpenditures,
				centralGovernment: centralGovernmentExpenditures,
				transferToRegional: transferToRegionalExpenditures,
			}
			state.economicMacros = {
				all: economicMacros,
				economicMacro: economicMacros,
            }
			state.progress = progress
			state.session = session
			state.specialPolicies = specialPolicies
			state.stateBudget = session.state_budget
			state.totalExpenditure = {
				all: totalAllExpenditures,
				centralGovernment: totalCentralGovernmentExpenditures,
				transferToRegional: totalTransferToRegionalExpenditures,
			}
			state.totalEconomicMacro = {
				all: totalAllEconomicMacro,
				economicMacro: totalEconomicMacroExpenditures,
            }
		},
	},
})

export const setEconomicMacroClientProgress = (done: boolean) =>
	slice.actions.setEconomicMacroClientProgress(done)
export const setCentralGovernmentClientProgress = (done: boolean) =>
	slice.actions.setCentralGovernmentClientProgress(done)
export const setTransferToRegionalClientProgress = (done: boolean) =>
	slice.actions.setTransferToRegionalClientProgress(done)
export const setSpecialPolicyClientProgress = (specialPolicyId: string, done: boolean) =>
	slice.actions.setSpecialPolicyClientProgress([specialPolicyId, done])
export const setClientSimulation = (simulationSession?: ClientSimulation) =>
	slice.actions.setClientSimulation(simulationSession)

export default slice.reducer
