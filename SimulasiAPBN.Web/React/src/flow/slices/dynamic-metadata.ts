/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { createSlice, PayloadAction, SliceCaseReducers } from "@reduxjs/toolkit"

import WebContent from "@core/models/WebContent"
import WebContentKey from "@core/enums/WebContentKey"
import DynamicMetadataState from "@flow/types/dynamic-metadata"

const slice = createSlice<DynamicMetadataState, SliceCaseReducers<DynamicMetadataState>>({
	name: "dynamic-metadata",
	initialState: {
		applicationTitle: "",
		contents: [],
		deficit: {
			law: "",
			threshold: 0,
		},
		grossDomesticProduct: 0,
		hashTag: "",
		invitationText: [],
		landingText: [],
		rules: [],
		videoUrl: "",
		disclaimer: "",
	},
	reducers: {
		setContents: (state, action: PayloadAction<WebContent[]>) => {
			state.contents = action.payload

			const titleWebContent = state.contents.find(webContent => webContent.key === WebContentKey.Title)
			if (typeof titleWebContent !== "undefined") {
				state.applicationTitle = titleWebContent.value
				document.title = state.applicationTitle
			}

			const hashTagWebContent = state.contents.find(webContent => webContent.key === WebContentKey.HashTag)
			if (typeof hashTagWebContent !== "undefined") {
				state.hashTag = hashTagWebContent.value
			}

			const invitationTextWebContent = state.contents.find(
				webContent => webContent.key === WebContentKey.InvitationText
			)
			if (typeof invitationTextWebContent !== "undefined") {
				state.invitationText = JSON.parse(invitationTextWebContent.value)
			}

			const landingTextWebContent = state.contents.find(
				webContent => webContent.key === WebContentKey.LandingText
			)
			if (typeof landingTextWebContent !== "undefined") {
				state.landingText = JSON.parse(landingTextWebContent.value)
			}

			const videoUrlWebContent = state.contents.find(webContent => webContent.key === WebContentKey.VideoUrl)
			if (typeof videoUrlWebContent !== "undefined") {
				let videoUrl = videoUrlWebContent.value
				if (!videoUrl.startsWith("http")) {
					const baseUrl = window.location.protocol + "//" + window.location.host
					videoUrl = videoUrl.startsWith("/") ? baseUrl + videoUrl : baseUrl + "/" + videoUrl
				}
				state.videoUrl = videoUrl
			}

			const disclaimer = state.contents.find(
				webContent => webContent.key === WebContentKey.Disclaimer)
			if (typeof disclaimer !== "undefined") {
				state.disclaimer = disclaimer.value
			}
		},
		setDeficitLaw: (state, action: PayloadAction<string>) => {
			state.deficit = { ...state.deficit, law: action.payload }
		},
		setDeficitThreshold: (state, action: PayloadAction<number>) => {
			state.deficit = { ...state.deficit, threshold: action.payload }
		},
		setGrossDomesticProduct: (state, action: PayloadAction<number>) => {
			state.grossDomesticProduct = action.payload
		},
		setRules: (state, action: PayloadAction<string[]>) => {
			state.rules = action.payload
		},
	},
})

export const setContents = (webContents: WebContent[] = []) => slice.actions.setContents(webContents)
export const setDeficitLaw = (law: string = "") => slice.actions.setDeficitLaw(law)
export const setDeficitThreshold = (threshold: number = 0) => slice.actions.setDeficitThreshold(threshold)
export const setGrossDomesticProduct = (grossDomesticProduct: number = 0) =>
	slice.actions.setGrossDomesticProduct(grossDomesticProduct)
export const setRules = (rules: string[] = []) => slice.actions.setRules(rules)

export default slice.reducer
