/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
export type CommonError = {
	message: string
	supportId?: string
}

export type CommonState = {
	applicationStarted: boolean
	darkMode: boolean
	displayContinueSession: boolean
	error?: CommonError
	loading: boolean | string
	networkLoading: boolean
	networkOnLine: boolean
}

export default CommonState
