/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { createSlice, PayloadAction, SliceCaseReducers } from "@reduxjs/toolkit"

import CommonState, { CommonError } from "@flow/types/common"

const slice = createSlice<CommonState, SliceCaseReducers<CommonState>>({
	name: "common",
	initialState: {
		applicationStarted: false,
		darkMode: false,
		displayContinueSession: true,
		loading: true,
		networkLoading: false,
		networkOnLine: "navigator" in window ? window.navigator.onLine : true,
	},
	reducers: {
		setApplicationStarted: (state, action: PayloadAction<boolean>) => {
			state.applicationStarted = action.payload
		},
		setDarkMode: (state, action: PayloadAction<boolean>) => {
			state.darkMode = action.payload
			if (action.payload) {
				document.documentElement.classList.add("dark")
			} else {
				document.documentElement.classList.remove("dark")
			}
		},
		setDisplayContinueSession: (state, action: PayloadAction<boolean>) => {
			state.displayContinueSession = action.payload
		},
		setError: (state, action: PayloadAction<CommonError | null>) => {
			state.error = action.payload === null ? void 0 : action.payload
		},
		setLoading: (state, action: PayloadAction<boolean | string>) => {
			state.loading = action.payload
		},
		setNetworkLoading: (state, action: PayloadAction<boolean>) => {
			state.networkLoading = action.payload
		},
		setNetworkOnLine: (state, action: PayloadAction<boolean>) => {
			state.networkOnLine = action.payload
		},
	},
})

export const clearError = () => slice.actions.setError(null)
export const setApplicationStarted = (applicationStarted: boolean = true) =>
	slice.actions.setApplicationStarted(applicationStarted)
export const setDarkMode = (darkMode: boolean) => slice.actions.setDarkMode(darkMode)
export const setDisplayContinueSession = (displayContinueSession: boolean = true) =>
	slice.actions.setDisplayContinueSession(displayContinueSession)
export const setError = (error: CommonError) => slice.actions.setError(error)

/*
 * string, falsify => Show Loading, No Text
 *
 * string, not empty => Show Loading, Show Text
 *
 * boolean, true => Show Loading, No Text
 *
 * boolean, false => No Loading
 * */
export const setLoading = (loading: boolean | string = true) => slice.actions.setLoading(loading)
export const setNetworkLoading = (networkLoading: boolean = true) => slice.actions.setNetworkLoading(networkLoading)
export const setNetworkOnLine = (networkOnLine: boolean = true) => slice.actions.setNetworkOnLine(networkOnLine)

export default slice.reducer
