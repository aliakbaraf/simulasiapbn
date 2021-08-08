/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
export type DynamicMetadataDeficit = {
	law: string
	threshold: number
}

export type DynamicMetadataState = {
	applicationTitle: string
	contents: WebContent[]
	deficit: DynamicMetadataDeficit
	grossDomesticProduct: number
	hashTag: string
	invitationText: string[]
	landingText: string[]
	rules: string[]
	videoUrl: string
	disclaimer: string
}

export default DynamicMetadataState
