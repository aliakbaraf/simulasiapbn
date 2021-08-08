/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { SimulationProgress } from "@flow/types/simulation"

export const getClientProgressDone = (progress: SimulationProgress) => {
	let specialPoliciesDone = true

	for (const key in progress.specialPoliciesDone) {
		if (!specialPoliciesDone) {
			break
		}

		if (Object.prototype.hasOwnProperty.call(progress.specialPoliciesDone, key)) {
			if (progress.specialPoliciesDone[key] === false) {
				specialPoliciesDone = false
			}
		}
	}

	return [progress.centralGovernmentDone, progress.transferToRegionalDone, specialPoliciesDone]
}

export const isAllClientProgressDone = (progress: SimulationProgress) => {
	const [centralGovernmentDone, transferToRegionalDone, specialPoliciesDone] = getClientProgressDone(progress)
	return centralGovernmentDone && transferToRegionalDone && specialPoliciesDone
}


export const isIncomeProgressDone = (progress: SimulationProgress) => {
	return progress.economicMacroDone
}
