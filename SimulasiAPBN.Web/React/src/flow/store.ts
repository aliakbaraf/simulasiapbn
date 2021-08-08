/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { combineReducers, configureStore } from "@reduxjs/toolkit"
import thunk from "redux-thunk"
import logger from "redux-logger"
import { PersistConfig, persistReducer, persistStore } from "redux-persist"
import { createInstance, INDEXEDDB, LOCALSTORAGE, WEBSQL } from "localforage"

import config from "@common/libraries/config"
import environment from "@common/libraries/environment"

import common from "@flow/slices/common"
import simulation from "@flow/slices/simulation"
import dynamicMetadata from "@flow/slices/dynamic-metadata"
import SimulationState from "@flow/types/simulation"
import DynamicMetadataState from "@flow/types/dynamic-metadata"
import RootState, { RootPersistPartial } from "@flow/types"

const persistConfig: PersistConfig<any> = {
	key: config.application.name,
	storage: createInstance({
		name: `${config.application.name}-data`,
		driver: [INDEXEDDB, WEBSQL, LOCALSTORAGE],
		version: 1.0,
	}),
}

const dynamicMetadataPersistConfig: PersistConfig<DynamicMetadataState> = {
	...persistConfig,
	key: "dynamic-metadata",
}

const simulationPersistConfig: PersistConfig<SimulationState> = {
	...persistConfig,
	key: "simulation",
}

const reducer = combineReducers<RootState & RootPersistPartial>({
	common: common,
	dynamicMetadata: persistReducer(dynamicMetadataPersistConfig, dynamicMetadata),
	simulation: persistReducer(simulationPersistConfig, simulation),
})

const store = configureStore({
	devTools: environment.isDevelopment(),
	middleware: environment.isDevelopment() ? [thunk, logger] : [thunk],
	reducer: reducer,
})

export const persistor = persistStore(store)

export default store
