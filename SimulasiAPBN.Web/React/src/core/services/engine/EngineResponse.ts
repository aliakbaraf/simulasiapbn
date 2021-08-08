/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import EngineError from "./EngineError"

interface EngineResponse<T> {
	success: boolean
	error?: EngineError
	data?: T
	message?: string
}

export default EngineResponse
