/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { PersistPartial } from "redux-persist/lib/persistReducer"

import SimulationState from "@flow/types/simulation"
import CommonState from "@flow/types/common"
import DynamicMetadataState from "@flow/types/dynamic-metadata"

export type RootState = {
	common: CommonState
	dynamicMetadata: DynamicMetadataState
	simulation: SimulationState
}

export type RootPersistPartial = {
	dynamicMetadata: PersistPartial
	simulation: PersistPartial
}

export default RootState
