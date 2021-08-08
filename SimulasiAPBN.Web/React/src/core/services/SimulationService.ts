/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import qs from "qs"

import WebContent from "@core/models/WebContent"
import ClientSimulation from "@core/types/ClientSimulation"
import Guid from "@core/types/Guid"

import Engine from "@core/services/engine/Engine"
import EngineUrl from "@core/services/engine/EngineUrl"

import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"

const engine = new Engine()
const toEndpoint = (endpoint: string) => new EngineUrl("/simulation" + endpoint)

export function newSession(name: string): Promise<ClientSimulation | undefined> {
	return engine.post(toEndpoint("/session"), qs.stringify({ name }))
}

export function getSession(sessionId: Guid): Promise<ClientSimulation | undefined> {
	return engine.get(toEndpoint("/session"), {
		params: { sessionId },
	})
}

export function finishSession(sessionId: Guid): Promise<ClientSimulation | undefined> {
	return engine.delete(toEndpoint("/session"), {
		params: { sessionId },
	})
}

export function updateSpecialPolicyAllocation(
	sessionId: Guid,
	simulationSpecialPolicyAllocations: SimulationSpecialPolicyAllocation[]
): Promise<ClientSimulation | undefined> {
	const data = simulationSpecialPolicyAllocations.map(({ id, total_allocation }) => ({
		id,
		totalAllocation: total_allocation,
	}))
	return engine.patch(toEndpoint("/specialPolicyAllocation"), qs.stringify({ data }), {
		params: { sessionId },
	})
}

export function updateStateExpenditure(
	sessionId: Guid,
	simulationStateExpenditures: SimulationStateExpenditure[]
): Promise<ClientSimulation | undefined> {
	const data = simulationStateExpenditures.map(({ id, total_allocation }) => ({
		id,
		totalAllocation: total_allocation,
	}))
	return engine.patch(toEndpoint("/stateExpenditure"), qs.stringify({ data }), {
		params: { sessionId },
	})
}

export function updateEconomicMacro(
	sessionId: Guid,
	simulationEconomicMacros: SimulationEconomicMacro[],
	newTotalEconomicMacro: number
): Promise<ClientSimulation | undefined> {
	const data = simulationEconomicMacros.map(({ id, used_value }) => ({
		id,
		usedValue: used_value,
	}))
	return engine.patch(toEndpoint("/economicMacro"), qs.stringify({ data }), {
		params: { sessionId, newTotalEconomicMacro },
	})
}

export function getWebContents(): Promise<WebContent[] | undefined> {
	return engine.get(toEndpoint("/webContent"))
}

export function getConfigDeficitLaw(): Promise<string | undefined> {
	return engine.get(toEndpoint("/config/deficitLaw"))
}

export function getConfigDeficitThreshold(): Promise<number | undefined> {
	return engine.get(toEndpoint("/config/deficitThreshold"))
}

export function getConfigGrossDomesticProduct(): Promise<number | undefined> {
	return engine.get(toEndpoint("/config/grossDomesticProduct"))
}

export function getRules(): Promise<string[] | undefined> {
	return engine.get(toEndpoint("/rules"))
}
