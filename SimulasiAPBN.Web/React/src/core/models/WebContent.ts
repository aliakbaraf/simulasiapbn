/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import GenericModel from "@core/models/GenericModel"
import WebContentKey from "@core/enums/WebContentKey"

interface WebContent extends GenericModel {
	key: WebContentKey
	value: string
}

export default WebContent
